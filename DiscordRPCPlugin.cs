using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins
{
    public class BaseCallbackResponse
    {
        public BaseCallbackResponse()
        {

        }

        public BaseCallbackResponse(string status, bool success)
        {
            this.status = status;
            this.success = success;
        }

        public string status { get; set; } = "success";
        public bool success { get; set; } = true;
    }

    public class SuccessCallbackResponse : BaseCallbackResponse
    {
        public SuccessCallbackResponse() : base("success", true)
        {
        }
    }

    public class ErrorCallbackResponse : BaseCallbackResponse
    {
        public ErrorCallbackResponse(string error) : base("error", false)
        {
            this.error = error;
        }

        public string error { get; set; }
    }

    public class OnClientReadyCallbackResponse : BaseCallbackResponse
    {
        public OnClientReadyCallbackResponse() : base("success", true)
        {
        }

        public User user { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DiscordRPCPlugin
    {
        private DiscordRpcClient client = null;
        public event Action<object> onClientReady;
        public event Action<object> onPresenceUpdate;
        public event Action<object> onClientError;
        public event Action<object> onLogLine;

        private Timestamps tsStartTime = null;
        private OverwolfConsoleLogger logger = null;
        private bool _initialize = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationID"></param>
        /// <param name="callback"></param>
        public void initialize(string applicationID, int logLevel, Action<object> callback)
        {            
            try
            {
                if (_initialize) {
                    throw new Exception("Already initialized");
                }

                tsStartTime = Timestamps.Now;

               //Logger logger = new ConsoleLogger();

                //if (!String.IsNullOrEmpty(logFilePath))
                //    logger = new FileLogger(logFilePath, (LogLevel)logLevel);

                logger = new OverwolfConsoleLogger((LogLevel)logLevel);
                logger.OnLogLine += Logger_OnLogLine;
                
                client = new DiscordRpcClient(applicationID, logger: logger);

                //Set the logger
                //client.Logger = new ConsoleLogger() { Level = DiscordRPC.Logging.LogLevel.Warning };

                //Subscribe to events
                client.OnReady += (sender, e) =>
                {
                    onClientReady?.Invoke(new OnClientReadyCallbackResponse() { user = e.User });
                };

                client.OnPresenceUpdate += (sender, e) =>
                {
                    onPresenceUpdate?.Invoke(new SuccessCallbackResponse());
                };

                client.OnError += (sendere, e) =>
                {
                    onClientError?.Invoke(new ErrorCallbackResponse(e.Message));
                };

                //Connect to the RPC
                client.Initialize();

                _initialize = true;

                callback(new SuccessCallbackResponse());
            }
            catch (Exception ex)
            {
                callback(new ErrorCallbackResponse(ex.Message));
            }
        }

        private void Logger_OnLogLine(LogMessage message)
        {
            onLogLine?.Invoke(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="details"></param>
        /// <param name="state"></param>
        /// <param name="largeImageKey"></param>
        /// <param name="largeImageText"></param>
        /// <param name="smallImageKey"></param>
        /// <param name="smallImageText"></param>
        /// <param name="callback"></param>
        public void updatePresence(string details, string state, string largeImageKey, string largeImageText, string smallImageKey, string smallImageText,
            bool showTimestamps, double endTime, string button1Text, string button1Url, string button2Text, string button2Url,
            Action<object> callback)
        {
            try
            {
                if (client == null)
                    callback(new ErrorCallbackResponse("DiscordRPCClient is null. Call initialize() first"));

                var activity = new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKey,
                        SmallImageKey = smallImageKey,
                        LargeImageText = largeImageText,
                        SmallImageText = smallImageText
                    },
                };

                if (showTimestamps)
                {
                    if (endTime == 0)
                        activity.Timestamps = tsStartTime;
                    else
                        activity.Timestamps = Timestamps.FromTimeSpan(endTime);
                }

                List<Button> buttons = new List<Button>();

                if (button1Text != "" && button1Url != "")
                {
                    buttons.Add(new Button()
                    {
                        Label = button1Text,
                        Url = button1Url
                    });
                }

                if (button2Text != "" && button2Url != "")
                {
                    buttons.Add(new Button()
                    {
                        Label = button2Text,
                        Url = button2Url
                    });
                }

                activity.Buttons = buttons.ToArray();

                client.SetPresence(activity);

                callback(new SuccessCallbackResponse());
            }
            catch (Exception ex)
            {
                callback(new ErrorCallbackResponse(ex.Message + " - " + ex.StackTrace));
            }
        }

        public void updatePresenceWithButtonsArray(string details, string state, string largeImageKey, string largeImageText, string smallImageKey, string smallImageText,
            bool showTimestamps, double endTime, string buttonsJson, Action<object> callback)
        {
            try
            {
                if (client == null)
                    callback(new ErrorCallbackResponse("DiscordRPCClient is null. Call initialize() first"));

                var activity = new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKey,
                        SmallImageKey = smallImageKey,
                        LargeImageText = largeImageText,
                        SmallImageText = smallImageText
                    },
                };

                if (showTimestamps)
                {
                    if (endTime == 0)
                        activity.Timestamps = tsStartTime;
                    else
                        activity.Timestamps = Timestamps.FromTimeSpan(endTime);
                }

                if (buttonsJson != null && buttonsJson != "")
                {
                    var buttons = Newtonsoft.Json.JsonConvert.DeserializeObject<Button[]>(buttonsJson);

                    activity.Buttons = ((Button[])buttons).ToArray();
                }

                client.SetPresence(activity);

                callback(new SuccessCallbackResponse());
            }
            catch (Exception ex)
            {
                callback(new ErrorCallbackResponse(ex.Message + " - " + ex.StackTrace));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void dispose(Action<object> callback)
        {
            try
            {
                if (client == null)
                    callback(new ErrorCallbackResponse("Call initialize first"));

                if (!client.IsDisposed)
                {
                    client.ClearPresence();
                    client.Dispose();
                }

                callback(new SuccessCallbackResponse());
                
            }
            catch (Exception ex)
            {
                callback(new ErrorCallbackResponse(ex.Message));
            } finally {
                _initialize = false;
            }
        }
    }
}
