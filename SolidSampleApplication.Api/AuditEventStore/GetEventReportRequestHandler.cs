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
    }

    public class GetEventReportRequestHandler : IQueryHandler<GetEventReportRequest, DefaultResponse>
    {
        private readonly ISimpleTableBuilder<SimpleReportAuditData, InputData> _reportTableBuilder;

        public GetEventReportRequestHandler(ISimpleTableBuilder<SimpleReportAuditData, InputData> reportTableBuilder)
        {
            _reportTableBuilder = reportTableBuilder;
        }

        public async Task<DefaultResponse> Handle(GetEventReportRequest request, CancellationToken cancellationToken)
        {
            var inputData = new InputData();
            var output = _reportTableBuilder.Build(inputData);
            return DefaultResponse.Success(output);
        }
    }

    public class ReportAuditTableBuilder : SimpleTableBuilderGeneric<SimpleReportAuditData, InputData>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;

        public ReportAuditTableBuilder(SimpleEventStoreDbContext eventStoreDbContext)
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

        protected override List<SimpleTableRow> GetDataAsSimpleTableRow(InputData input)
        {
            var newList = new List<SimpleTableRow>();
            var applicationEvents = _eventStoreDbContext.ApplicationEvents.ToList();
            applicationEvents.ForEach((ae) =>
            {
                newList.Add(Membership.SimpleReportAuditData.FromSimpleApplicationEvent(ae).ToSimpleTableRow());
            });
            return newList;
        }
    }

    public class SimpleReportAuditData
    {
        public static SimpleReportAuditData FromSimpleApplicationEvent(SimpleApplicationEvent evt)
        {
            // parse here if we need to parse some values
            var dt = new SimpleReportAuditData();
            var actionType = evt.EventType.Split(',').First().Split('.').LastOrDefault();
            dt.Action = actionType switch
            {
                "CustomerRegisteredEvent" => "Customer Registration",
                "MembershipPointsEarnedEvent" => "Points Earned",
                null => "Empty",
                _ => actionType
            };
            dt.ActionTarget = evt.AggregateId;
            dt.ActionTargetValue = evt.EventData;
            dt.ActionedBy = evt.RequestedBy;
            dt.ActionedDateAndTime = evt.RequestedTime;
            return dt;
        }

        public string Action { get; set; }
        public string ActionTarget { get; set; }
        public string ActionTargetValue { get; set; }
        public string ActionedBy { get; set; }
        public DateTime ActionedDateAndTime { get; set; }

        public SimpleTableRow ToSimpleTableRow()
        {
            var tableRow = new SimpleTableRow();
            tableRow.Cells = new List<string> { Action, ActionTarget, ActionTargetValue, ActionedBy, ActionedDateAndTime.ToString() };
            return tableRow;
        }
    }

    public class InputData
    {
        public string EmployeeNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}