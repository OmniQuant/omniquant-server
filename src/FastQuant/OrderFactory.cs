﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace FastQuant
{ 
    public class OrderFactory
    {
        private readonly IdArray<Order> orders = new IdArray<Order>(1024000);

        public Order OnExecutionCommand(ExecutionCommand command)
        {
            var order = this.orders[command.OrderId];
            if (order == null)
            {
                order = new Order(command);
                this.orders[command.OrderId] = order;
                order.Instrument = command.Instrument;
                order.Provider = command.Provider;
                order.Portfolio = command.Portfolio;
                if (command.Type == ExecutionCommandType.Send)
                    order.Status = OrderStatus.PendingNew;
            }
            order.OnExecutionCommand(command);
            return order;
        }

        public Order OnExecutionReport(ExecutionReport report)
        {
            var order = this.orders[report.OrderId];
            if (order == null)
                return null;
            report.Order = order;
            order.OnExecutionReport(report);
            return order;
        }

        public void Reset() => this.orders.Clear();
    }
}
