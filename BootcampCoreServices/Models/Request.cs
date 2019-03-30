using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BootcampCoreServices
{
    [Serializable, XmlRoot("requests")]
    public class Requests
    {
        [XmlElement("request")]
        public List<Request> requests { get; set; }
        
    }
    public class Request
    {
        [StringLength(6)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        [XmlElement(ElementName = "clientId")]
        public string ClientId { get; set; }
        [XmlElement(ElementName = "requestId")]
        public long RequestId { get; set; }
        [StringLength(255)]
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "quantity")]
        public int Quantity { get; set; }
        [XmlElement(ElementName = "price")]
        public double Price { get; set; }
    }
}
