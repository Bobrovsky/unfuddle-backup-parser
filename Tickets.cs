using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;

using OfficeOpenXml;

namespace UnfuddleBackupParser
{
    class Tickets
    {
        struct Ticket
        {
            public string id;
            public string summary;
            public string description;
            public string assigneeId;
            public string dueOn;
            public string reporterId;
            public string priority;
            public string createdAt;
            public string milestoneId;
            public string componentId;
            public Attachments attachments;
            public Events events;
        }

        SortedDictionary<int, Ticket> m_tickets;

        public Tickets(XmlElement element)
        {
            m_tickets= new SortedDictionary<int, Ticket>();
            parse(element);
        }

        public void Save(ExcelWorksheet worksheet,
            string project, Components components, Milestones milestones,
            People people, bool cleanupEvents)
        {
            int row = 1;

            worksheet.Column(7).Width = 50;
            worksheet.Column(8).Width = 50;

            saveLine(worksheet, row, "cmd", "dt", "sProject", "sArea",
                "sFixFor", "ixPriority", "sTitle", "sEvent", "sPersonAssignedTo",
                "dtDue", "reporter", "attachments");
            row++;

            foreach (KeyValuePair<int, Ticket> pair in m_tickets)
            {
                Ticket ticket = pair.Value;

                string area = getComponent(components, ticket.componentId);
                string milestone = getMilestone(milestones, ticket.milestoneId);

                foreach (KeyValuePair<DateTime, Events.Event> eventPair in ticket.events)
                {
                    Events.Event e = eventPair.Value;
                    
                    string eventString;
                    switch (e.eventType)
                    {
                        case "accept":
                        case "command":
                        case "unaccept":
                        case "update":
                        case "reassign":
                        case null:
                        default:
                            eventString = "edit";
                            break;

                        case "close":
                            eventString = "close";
                            break;

                        case "create":
                            eventString = "new";
                            break;

                        case "resolve":
                            eventString = "resolve";
                            break;
                        
                        case "reopen":
                            eventString = "reopen";
                            break;
                    }

                    if (eventString == "new")
                    {
                        string assignee = people.GetPersonName(ticket.assigneeId);
                        string reporter = people.GetPersonName(ticket.reporterId);
                        string attachments = null;
                        if (ticket.attachments != null)
                            attachments = ticket.attachments.GetAllAttachments();

                        saveLine(worksheet, row,
                                eventString, ticket.createdAt, project, area, milestone,
                                convertPriority(ticket.priority), ticket.summary, ticket.description,
                                assignee, ticket.dueOn, reporter, attachments);

                        row++;
                    }
                    else
                    {
                        string attachments = null;
                        if (e.attachments != null)
                            attachments = e.attachments.GetAllAttachments();

                        string eventReporter = people.GetPersonName(e.personId);
                        string description = e.description;
                        bool skipLine = false;
                        if (cleanupEvents && (eventString == "edit" || eventString == "close" || eventString == "reopen") && e.eventType != null)
                        {
                            if (string.IsNullOrEmpty(attachments) && eventString == "edit")
                                skipLine = true;

                            description = null;
                        }

                        if (!skipLine)
                        {
                            saveLine(worksheet, row,
                                    eventString, e.createdAt, null, null, null,
                                    null, null, description, null, null,
                                    eventReporter, attachments);

                            row++;
                        }
                    }
                    
                }
            }
        }

        private static string convertPriority(string unfuddlePriority)
        {
            int priority = 1;
            switch (unfuddlePriority)
            {
                case "5":
                    priority = 1;
                    break;
                case "4":
                    priority = 2;
                    break;
                case "3":
                    priority = 3;
                    break;
                case "2":
                    priority = 4;
                    break;
                case "1":
                    priority = 5;
                    break;
            }

            return priority.ToString();
        }

        private static string getComponent(Components components, string id)
        {
            string component = null;
            if (components != null)
                component = components.GetComponentName(id);

            return component;
        }

        private static string getMilestone(Milestones milestones, string id)
        {
            string milestone = null;
            if (milestones != null)
                milestone = milestones.GetMilestoneName(id);

            return milestone;
        }

        private static void saveLine(ExcelWorksheet worksheet, int row,
            string command, string time, string project, string area, string milestone,
            string priority, string title, string description, string assignedTo,
            string dueOn, string reporterId, string attachments)
        {
            int col = 1;

            worksheet.Cells[row, col++].Value = command;
            worksheet.Cells[row, col++].Value = time;
            worksheet.Cells[row, col++].Value = project;
            worksheet.Cells[row, col++].Value = area;
            worksheet.Cells[row, col++].Value = milestone;
            worksheet.Cells[row, col++].Value = priority;
            worksheet.Cells[row, col++].Value = title;
            worksheet.Cells[row, col++].Value = description;
            worksheet.Cells[row, col++].Value = assignedTo;
            worksheet.Cells[row, col++].Value = dueOn;
            worksheet.Cells[row, col++].Value = reporterId;
            worksheet.Cells[row, col++].Value = attachments;
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Ticket ticket = new Ticket();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string value = HttpUtility.HtmlDecode(child.InnerText);
                    switch (child.Name)
                    {
                        case "id":
                            ticket.id = value;
                            break;

                        case "summary":
                            ticket.summary = value;
                            break;

                        case "description":
                            ticket.description = value;
                            break;

                        case "assignee-id":
                            ticket.assigneeId = value;
                            break;

                        case "due-on":
                            ticket.dueOn = value;
                            break;

                        case "reporter-id":
                            ticket.reporterId = value;
                            break;

                        case "priority":
                            ticket.priority = value;
                            break;

                        case "created-at":
                            ticket.createdAt = value;
                            break;

                        case "milestone-id":
                            ticket.milestoneId = value;
                            break;

                        case "attachments":
                            if (child.ChildNodes.Count != 0)
                                ticket.attachments = new Attachments(child as XmlElement);
                            break;

                        case "component-id":
                            ticket.componentId = value;
                            break;

                        case "audit-trails":
                        case "comments":
                            if (child.ChildNodes.Count != 0)
                            {
                                if (ticket.events == null)
                                    ticket.events = new Events();

                                ticket.events.Parse(child as XmlElement);
                            }
                            break;
                    }
                }

                m_tickets.Add(Convert.ToInt32(ticket.id), ticket);
            }
        }
    }
}
