﻿using Azure.Messaging;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PostReportController : BaseController<PostReportController>
    {
        private readonly IPostReportService _postReportService;

        public PostReportController(ILogger<PostReportController> logger, IPostReportService postReportService) : base(logger)
        {
            _postReportService = postReportService;
        }

        [HttpGet(ApiEndPointConstant.PostReport.PostReportEndPoint)]
        [ProducesResponseType(typeof(PostReportReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportById(int id)
        {
            var response = await _postReportService.GetReportById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.PostReport.PostReportsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewReport([FromBody] PostReportFilter request)
        {
            var id = await _postReportService.CreateNewReport(request);
            return CreatedAtAction(nameof(PostReportReponse), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.PostReport.PostReportsEndPoint)]
        [ProducesResponseType(typeof(PostReportReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllReport([FromQuery] PostReportFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _postReportService.ViewAllReport(filter, pagingModel);
            return Ok(response);
        }
    }
}