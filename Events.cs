using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Web;

namespace UnfuddleBackupParser
{
    class Events : IEnumerable
    {
        public struct Event
        {
            public string id;
            public string eventType;
            public string createdAt;
            public string personId;
            public string description;
            public Attachments attachments;
        }

        SortedDictionary<DateTime, Event> m_events;

        public Events()
        {
            m_events = new SortedDictionary<DateTime, Event>();
        }

        public IEnumerator GetEnumerator()
        {
            return m_events.GetEnumerator();
        }

        public void Parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Event e = new Event();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string value = HttpUtility.HtmlDecode(child.InnerText);
                    switch (child.Name)
                    {
                        case "id":
                            e.id = value;
                            break;

                        case "created-at":
                            e.createdAt = value;
                            break;

                        case "description":
                        case "body":
                            e.description = value.Trim();
                            break;

                        case "event":
                            e.eventType = value;
                            break;

                        case "person-id":
                        case "author-id":
                            e.personId = value;
                            break;

                        case "summary":
                            if (string.IsNullOrEmpty(e.description))
                                e.description = value.Trim();
                            break;

                        case "attachments":
                            e.attachments = new Attachments(child as XmlElement);
                            break;
                    }
                }

                DateTime dt = Convert.ToDateTime(e.createdAt);
                while (m_events.ContainsKey(dt))
                    dt = dt.AddSeconds(1);

                m_events.Add(dt, e);
            }
        }
    }
}
