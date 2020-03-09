﻿using Microsoft.AspNetCore.Mvc;
using System;

namespace SolidSampleApplication.Api.Membership
{
    public class DefaultResponse
    {
        public static DefaultResponse Success(object output = null) => new DefaultResponse(new OkObjectResult(output));

        public static DefaultResponse Failed(object requestObject, Exception e) => new DefaultResponse(new BadRequestObjectResult(requestObject), 1, e.Message, false);

        public ActionResult ActionResult { get; private set; }
        public int ErrorId { get; private set; }
        public string ErrorDescription { get; private set; }
        public bool IsSuccess { get; private set; }

        protected DefaultResponse(ActionResult actionResult, int errorId = -1, string errorDescription = "", bool isSuccess = true)
        {
            ActionResult = actionResult ?? throw new ArgumentNullException(nameof(actionResult));
            ErrorId = errorId;
            ErrorDescription = errorDescription ?? throw new ArgumentNullException(nameof(errorDescription));
            IsSuccess = isSuccess;
        }
    }
}