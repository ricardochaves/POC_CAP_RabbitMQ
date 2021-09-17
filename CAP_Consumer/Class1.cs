using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CAP_Consumer
{
    public class MessageTable
    {
        public int MessageId { get; set; }
        public string ConsumerId { get; set; }
        public DateTime CreateAt { get; set; }
    }
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }

        public DbSet<MessageTable> MessageTables { get; set; }
    }
    public class PKGMessage<T>
    {
        private T data;
        private readonly SystemContext _context;

        public PKGMessage(T _data, SystemContext db)
        {
            data = _data;
            _context = db;
        }
        public PKGMessage(T _data)
        {
            data = _data;
        }

        public T Data
        {
            get => data;
            set => data = value;
        }

        public async Task Commit()
        {
            _context.MessageTables.Add(new MessageTable()
            {
                ConsumerId = "consumerId",CreateAt = DateTime.Now,MessageId = 1
            });
            await _context.SaveChangesAsync();
        }
    }

    public static class Consumer
    {
        public static async Task ExecuteMessage<T>(T message, Action<PKGMessage<T>> execute)
        {
            var x = new PKGMessage<T>(message);
            execute(x);
        }
    }
}
