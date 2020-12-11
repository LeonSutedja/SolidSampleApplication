using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.TableEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetEventReportRequest : IQuery<DefaultResponse>
    {
        public bool AsPdf { get; init; }

        public GetEventReportRequest(bool asPdf)
        {
            AsPdf = asPdf;
        }
    }

    public class GetEventReportRequestHandler : IQueryHandler<GetEventReportRequest, DefaultResponse>
    {
        private readonly ISimpleTableBuilder<SimpleApplicationEvent, InputData> _reportTableBuilder;

        public GetEventReportRequestHandler(ISimpleTableBuilder<SimpleApplicationEvent, InputData> reportTableBuilder)
        {
            _reportTableBuilder = reportTableBuilder;
        }

        public async Task<DefaultResponse> Handle(GetEventReportRequest request, CancellationToken cancellationToken)
        {
            var inputData = new InputData();
            if(request.AsPdf)
            {
                var fileContents = _reportTableBuilder.GenerateFileContentsPdf(inputData);
                return DefaultResponse.SuccessAsFile(fileContents);
            }
            var output = _reportTableBuilder.Build(inputData);
            return DefaultResponse.Success(output);
        }
    }

    public class ReportAuditTableBuilder : SimpleTableBuilderGeneric<SimpleApplicationEvent, InputData>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;

        public ReportAuditTableBuilder(SimpleEventStoreDbContext eventStoreDbContext, IPdfGeneratorWrapper pdfGeneratorWrapper) : base(pdfGeneratorWrapper)
        {
            _eventStoreDbContext = eventStoreDbContext;
        }

        protected override List<SimpleColumnDefinition> GetColumnDefinitionsForReport(InputData input)
        {
            var tableColumns = new List<SimpleColumnDefinition>();
            tableColumns.Add(SimpleColumnDefinition.SimpleString("Action", "ACTTP", 1));
            tableColumns.Add(SimpleColumnDefinition.SimpleString("Action Target", "ACTTG", 2));
            tableColumns.Add(SimpleColumnDefinition.SimpleString("Action Target Value", "ACTVL", 3));
            tableColumns.Add(SimpleColumnDefinition.SimpleString("Actioned by", "ACTBY", 4));
            tableColumns.Add(SimpleColumnDefinition.SimpleString("Actioned Date & Time", "ACTDT", 5));
            return tableColumns;
        }

        protected override IEnumerable<SimpleApplicationEvent> GetDataAsEntity(InputData input)
            => _eventStoreDbContext.ApplicationEvents.ToList().AsEnumerable();

        protected override SimpleTableRow MapToTableRow(SimpleApplicationEvent evt)
        {
            var actionType = evt.EventType.Split(',').First().Split('.').LastOrDefault();
            var action = actionType switch
            {
                "CustomerRegisteredEvent" => "Customer Registration",
                "MembershipPointsEarnedEvent" => "Points Earned",
                null => "Empty",
                _ => actionType
            };
            var actionTarget = evt.AggregateId;
            var actionTargetValue = evt.EventData;
            var actionedBy = evt.RequestedBy;
            var actionedDateAndTime = evt.RequestedTime;
            var tableRow = new SimpleTableRow()
            {
                Cells = new List<string> { action, actionTarget, actionTargetValue, actionedBy, actionedDateAndTime.ToString() }
            };
            return tableRow;
        }
    }

    public class InputData
    {
        public string CustomerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}