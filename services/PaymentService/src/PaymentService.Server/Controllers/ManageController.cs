﻿using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Server.Controllers;

[ApiController]
[Route("/manage")]
public class ManageController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok();
    }
}