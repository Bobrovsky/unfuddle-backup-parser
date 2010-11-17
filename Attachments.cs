using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace UnfuddleBackupParser
{
    class Attachments
    {
        struct Attachment
        {
            public string id;
            public string name;
            public string nameOnDisk;
        }

        SortedDictionary<int, Attachment> m_attachments;

        public Attachments(XmlElement element)
        {
            m_attachments= new SortedDictionary<int, Attachment>();
            parse(element);
        }

        public string GetAllAttachments()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, Attachment> pair in m_attachments)
            {
                Attachment a = pair.Value;
                sb.Append(a.name);
                sb.Append(";");
                sb.Append(a.nameOnDisk);
                sb.Append(";");
            }

            return sb.ToString();
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Attachment attachment = new Attachment();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string value = HttpUtility.HtmlDecode(child.InnerText);
                    switch (child.Name)
                    {
                        case "id":
                            attachment.id = value;
                            attachment.nameOnDisk = "media\\" + value;
                            break;

                        case "filename":
                            attachment.name = value;
                            break;
                    }
                }

                m_attachments.Add(Convert.ToInt32(attachment.id), attachment);
            }
        }
    }
}
