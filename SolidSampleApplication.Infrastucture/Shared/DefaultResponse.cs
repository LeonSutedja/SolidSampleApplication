using Microsoft.AspNetCore.Mvc;
using System;

namespace SolidSampleApplication.Infrastructure.Shared
{
    public class DefaultResponse
    {
        public static DefaultResponse Success(object output = null) => new DefaultResponse(new OkObjectResult(output));

        public static DefaultResponse Failed(object requestObject, Exception e = null)
        {
            if(e != null)
                return new DefaultResponse(new BadRequestObjectResult(requestObject), 1, e.Message, false);

            return new DefaultResponse(new BadRequestObjectResult(requestObject), 1, string.Empty, false);
        }

        public ActionResult ActionResult { get; private set; }
        public int ErrorId { get; private set; }
        public string ErrorDescription { get; private set; }
        public bool IsSuccess { get; private set; }

        protected DefaultResponse(ActionResult actionResult, int errorId = -1, string errorDescription = "", bool isSuccess = true)
        {
            ActionResult = actionResult ?? throw new ArgumentNullException(nameof(actionResult));
            ErrorId = errorId;
            ErrorDescription = errorDescription;
            IsSuccess = isSuccess;
        }
    }
}