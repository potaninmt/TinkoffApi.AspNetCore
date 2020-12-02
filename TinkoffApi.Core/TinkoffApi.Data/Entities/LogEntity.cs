using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinkoffApi.Data.Enums;
namespace TinkoffApi.Data.Entities
{
    [Table("Log")]
    public class LogEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// тип события
        /// </summary>
        public EventType? eventType { get; set; }

        /// <summary>
        /// информация о событии
        /// </summary>
        public string info { get; set; }

        /// <summary>
        /// время события
        /// </summary>
        public DateTime eventDt { get; set; }
    }
}
