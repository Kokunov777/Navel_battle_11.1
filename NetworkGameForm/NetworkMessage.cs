using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGame
{
    [Serializable]
    public class NetworkMessage
    {
        public MessageType Type { get; set; } // Тип сообщения
        public object Data { get; set; }     // Данные сообщения

        public enum MessageType
        {
            ShipPlacement, // Размещение корабля
            Attack,        // Атака
            GameState,     // Состояние игры
            ChatMessage   // Сообщение в чат
        }
    }
}
