using Microsoft.AspNetCore.Mvc;
using System;

namespace SolidSampleApplication.Infrastructure.Shared
{
    public class DefaultResponse
    {
        public static DefaultResponse Success(object output = null) => new DefaultResponse(new OkObjectResult(output));

        public static DefaultResponse SuccessAsFile(byte[] fileContents)
            => new DefaultResponse(new FileContentResult(fileContents, "application/pdf"));

        public static DefaultResponse Failed(object requestObject, Exception e = null)
        {
            if(e != null)
            {
                var errorObject = new ErrorObject(requestObject, 1, e.Message);
                return new DefaultResponse(new BadRequestObjectResult(errorObject), false);
            }

            return new DefaultResponse(new BadRequestObjectResult(requestObject), false);
        }

        public static DefaultResponse Failed(object requestObject, string message)
        {
            var errorObject = new ErrorObject(requestObject, 1, message);
            return new DefaultResponse(new BadRequestObjectResult(errorObject), false);
        }

        public ActionResult ActionResult { get; private set; }
        public bool IsSuccess { get; private set; }

        protected DefaultResponse()
        {
        }

        protected DefaultResponse(ActionResult actionResult, bool isSuccess = true)
        {
            ActionResult = actionResult ?? throw new ArgumentNullException(nameof(actionResult));
            IsSuccess = isSuccess;
        }

        public class ErrorObject
        {
            public object RequestObject { get; init; }
            public int ErrorId { get; init; }
            public string ErrorDescription { get; init; }

            public ErrorObject(object requestObject, int errorId, string errorDescription)
            {
                RequestObject = requestObject ?? throw new ArgumentNullException(nameof(requestObject));
                ErrorId = errorId;
                ErrorDescription = errorDescription ?? throw new ArgumentNullException(nameof(errorDescription));
            }
        }
    }
}