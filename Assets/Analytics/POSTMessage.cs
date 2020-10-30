using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Analytics
{
   public class POSTMessage
    {
        public string type { get;private set; }
        public object data { get;private set; }

        private const string assemblyPath = "Assets.Analytics.";

        public POSTMessage(string type,params object[] args)
        {
            this.type = type;
            Type messageType = Type.GetType(assemblyPath+type, false, true);
            data = Activator.CreateInstance(messageType);
            if(data is ICustomParams && args != null)
            {
                MethodInfo method = messageType.GetMethod("AddMoreInfo");
                method.Invoke(data, args);
            }
        }

    }

    public class level_start : MessageData, ILevelEvent
    {
        public int level { get;  set; } 
        public level_start()
        {
            level = Application.loadedLevel;
        }
    }

    public class level_win : MessageData, ILevelEvent
    {
        public int level { get;  set; }
        public level_win()
        {
            level = Application.loadedLevel;
        }
    }
    public class level_fail : MessageData, ILevelEvent
    {
        public int level { get; set; }
        public level_fail() 
        {
            level = Application.loadedLevel;
        }
    }
    public class pay_in_game : MessageData, IPayEvent, ICustomParams
    {
       
        public int price { get ; set ; }
        public string id { get; set ; }

        public void AddMoreInfo(object[] args)
        {
            price = Convert.ToInt32(args[0]);
            id = args[1].ToString();
        }
    }


    public class MessageData
    {
        public long time { get; private set; }
        public string version { get; private set; }

        public MessageData()
        {
            time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            version = Application.version;
        }
    }

    /// <summary>
    /// This class contains string event -  identification for server
    /// </summary>
    public static class EventsType
    {
        public const string LevelStart = "level_start";
        public const string LevelWin = "level_win";
        public const string LevelFail = "level_fail";
        public const string PayInGame = "pay_in_game";

        
    }

    public interface ILevelEvent
    {
         int level { get;   set; }
    }

    public interface IPayEvent
    {
        int price { get; set; }
        string id { get; set; }

      
    }

    public interface ICustomParams
    {
        void AddMoreInfo(object[] args);
    }
}
