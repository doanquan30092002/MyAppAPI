﻿using MediatR;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket
{
    public class UpdateStatusTicketRequest : IRequest<bool>
    {
        public Guid AuctionDocumentsId { get; set; }
    }

    public class SePayWebhook
    {
        public int Id { get; set; }
        public string Gateway { get; set; }
        public string TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public string TransferType { get; set; }
        public decimal TransferAmount { get; set; }
        public decimal Accumulated { get; set; }
        public string SubAccount { get; set; }
        public string ReferenceCode { get; set; }
        public string Description { get; set; }
    }
}
