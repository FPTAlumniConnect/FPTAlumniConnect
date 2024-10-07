﻿using Azure.Messaging;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.Post;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PostController : BaseController<PostController>
    {
        private readonly IPostService _postService;

        public PostController(ILogger<PostController> logger, IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        [HttpGet(ApiEndPointConstant.Post.PostEndPoint)]
        [ProducesResponseType(typeof(PostReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostById(int id)
        {
            var response = await _postService.GetPostById(id);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Post.PostEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewPost([FromBody] PostInfo request)
        {
            var id = await _postService.CreateNewPost(request);
            return CreatedAtAction(nameof(PostReponse), new { id }, id);
        }
        [HttpGet(ApiEndPointConstant.Post.PostsEndPoint)]
        [ProducesResponseType(typeof(PostReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllUser([FromQuery] PostFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _postService.ViewAllPost(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Post.PostsEndPoint)]
        [ProducesResponseType(typeof(PostReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserInfo(int id, PostInfo request)
        {
            var isSuccessful = await _postService.UpdatePostInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}