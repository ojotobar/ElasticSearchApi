using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.DTO;
using ElasticSearchApi.Entities.Enums;
using ElasticSearchApi.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using RedisCache.Common.Repository;

namespace ElasticSearch.Api.Controllers
{
    [ApiController]
    [Route("api/v1/posts")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IRepositoryManager repository;
        private readonly ICacheCommonRepository cacheRepository;

        public PostController(
            ILogger<PostController> logger, 
            IRepositoryManager repository,
            ICacheCommonRepository cacheRepository
            )
        {
            _logger = logger;
            this.repository = repository;
           this.cacheRepository = cacheRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostForCreate request)
        {
            var post = new Post
            {
                Message = request.Message,
                User = request.User,
            };
            _logger.LogInformation("Adding a new post");
            await repository.Post.AddAsync(post);
            await cacheRepository.SetAsync($"{nameof(Post)}_{post.Id}", post, DateTimeOffset.UtcNow.AddMinutes(5));
            repository.PostPubSub.PublishPost(post, ERabbitExchanges.AppLogger, ERabbitQueues.PostLog);
            repository.PostPubSub.SubscribeToPost(ERabbitQueues.PostLog);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SearchParameters search)
        {
            var result = await repository.Post.FindAsync(search);

            var count = await repository.Post.CountAsync(search);
            _logger.LogInformation($"Processing {count} posts returned from the database");
            var res = PagedList<Post>.ToPagedList(result, count, search.PageNumber, search.PageSize);

            return Ok(PaginatedListDto<Post>.Paginate(res, res.MetaData));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await cacheRepository.GetAsync<Post>($"{nameof(Post)}_{id}");
            if(post == null)
            {
                post = await repository.Post.FindAsync(id);
                if (post == null)
                {
                    _logger.LogError($"Post with id: {id} not found");
                    return BadRequest($"Post with id: {id} not found");
                } 
            }

            return Ok(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] PostForCreate request)
        {
            var post = await repository.Post.FindAsync(id);
            if (post != null)
            {
                post.Message = request.Message;
                post.User = request.User;
                await cacheRepository.RemoveAsync($"{nameof(Post)}_{id}");
                return Ok(await repository.Post.UpdateAsync(id, post));
            }
            _logger.LogError($"Post with id: {id} not found");
            return BadRequest($"Post with id: {id} not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var post = await repository.Post.FindAsync(id);
            if(post != null)
            {
                var isSuccess = await repository.Post.DeleteAsync(id);
                await cacheRepository.RemoveAsync($"{nameof(Post)}_{id}");
                return Ok(isSuccess);
            }
            _logger.LogError($"Post with id: {id} not found");
            return BadRequest($"Post with id: {id} not found");
        }
    }
}