#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;

namespace UITrial
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        string location = null;
        string tempType = null;
        string time, condition_day0;
        private string periodicTaskName = "LiveTilePeriodicAgent";
        private PeriodicTask periodicTask;
        /// Holds the push channel that is created or found.
        HttpNotificationChannel pushChannel;
        // The name of our push channel.
        string channelName = "LiveTileChannel";

        public MainPage()
        {
            InitializeComponent();
            Sun.Opacity = Sun_today.Opacity = Sun_tomm.Opacity = Sun_dayafter.Opacity = 0;

            cloud.Opacity = cloud_today.Opacity = cloud_tomm.Opacity = cloud_dayafter.Opacity = 0;
            
            cloud___Copy__2_.Opacity = cloud___Copy__2__today.Opacity = cloud___Copy__2__tomm.Opacity = cloud___Copy__2__dayafter.Opacity = 0;     
           
            moonlayer.Opacity = 0;
           
            lightning1.Opacity = lightning2.Opacity = lightning3.Opacity = lightning.Opacity = lightning_today.Opacity =
                lightning_tomm.Opacity = lightning_dayafter.Opacity = 0;
            
            raind1.Opacity = raind2.Opacity = raind3.Opacity = raind4.Opacity = raind5.Opacity = 0;
           
            drops_today.Opacity = drops_tomm.Opacity = drops_dayafter.Opacity = 0;

           textBlock4.Text= textBlock5.Text= textBlock7.Text = textBlock8.Text = textBlock10.Text = textBlock11.Text = "";

           snowflake1.Opacity = snowflake2.Opacity = snowflake3.Opacity = snowflake4.Opacity = snowflake5.Opacity = 0;

           snow_today.Opacity = snow_tomm.Opacity = snow_day_after.Opacity = 0;

           CreatePushChannel();
        }

       

        // LIVE TILE CODE ADDED HERE !!!!!!!!------------------------!!!!!!!!!!!!!!!!

        private void CreatePushChannel()
        {
            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                pushChannel.Open();

                // Bind this new channel for Tile events.
                pushChannel.BindToShellTile();
            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
   // hereeee uncommnt      //       System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}", pushChannel.ChannelUri.ToString()));
            }
        }


        /// <summary>
        /// Event handler for when the Push Channel Uri changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                // Display the new URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}", e.ChannelUri.ToString()));
            });
        }

        /// <summary>
        /// Event handler for when a Push Notification error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}", e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData)));
        }

        private void updateTile_Click_1(object sender, RoutedEventArgs e)
        {
          
        }
        void getTime()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted2);
            client.DownloadStringAsync(new Uri("http://www.worldweatheronline.com/feed/tz.ashx?key=bed6524371124406111310&q=" + location + "&format=xml")); // weather location              
            
        }
        void client_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Network Error");

            }
            else
            {
                XElement XmlTime = XElement.Parse(e.Result);
                string localTime = XmlTime.ToString();
                if (localTime.Contains("error"))
                {
             //       MessageBox.Show("Invalid Location");
                }
                else
                {
                    int start, end;
                    start = localTime.IndexOf("<localtime>") + 11 + 11;//extra 11 is to avoid the date in xml
                    end = localTime.IndexOf("</localtime>");
                    time = localTime.Substring(start, end - start);
                    
                    {
                        int x = Int32.Parse(time.Substring(0, 2));

                        if (condition_day0 != null)
                        {
                            if (condition_day0.Contains("Clear"))
                            {
                                if ((x >= 19) || (x <= 6))
                                {
                                    Sun.Opacity = 0;
                                    moonlayer.Opacity = 100;
                                    cloud.Opacity = 10;

                                }
                            }
                        }
                    }
                }
            }
        }
       

        void showConditions(string conditions, int day)
        {
          
            if (conditions.Contains("cloud") || (conditions.Contains("Cloud") ))
            {
                if (day == 0)
                {
                    cloud___Copy__2_.Opacity = 100;
                    cloud.Opacity = 100;
                    cloudGlow.Begin();
                    cloudGlow.RepeatBehavior = RepeatBehavior.Forever;
                }
                else if (day == 1)
                {
                    cloud___Copy__2__today.Opacity = 100;
                    cloud_today.Opacity = 100;                  
                }
                else if (day == 2)
                {
                    cloud___Copy__2__tomm.Opacity = 100;
                    cloud_tomm.Opacity = 100;
                }
                else if (day == 3)
                {
                    cloud___Copy__2__dayafter.Opacity = 100;
                    cloud_dayafter.Opacity = 100;
                }

            }
                if(conditions.Contains("rain") || (conditions.Contains("Rain")))                  
                {
                    if (day == 0)
                    {
                        cloud.Opacity = 50;
                        cloud___Copy__2_.Opacity = 100;
                        lightning1.Opacity = 100;
                        lightning2.Opacity = 18;
                        lightning3.Opacity = 0;
                        lightningShine.Begin();
                        lightningShine.RepeatBehavior = RepeatBehavior.Forever;
                        raining.Begin();
                        raining.RepeatBehavior = RepeatBehavior.Forever;
                    }
                    else if (day == 1)
                    {
                        cloud_today.Opacity = 100;
                        cloud___Copy__2__today.Opacity = 100;
                        lightning_today.Opacity = 100;
                        drops_today.Opacity = 100;
                    }
                    else if (day == 2)
                    {
                        cloud_tomm.Opacity = 100;
                        cloud___Copy__2__tomm.Opacity = 100;
                        lightning_tomm.Opacity = 100;
                        drops_tomm.Opacity = 100;
                    }
                    else if (day == 3)
                    {
                        cloud_dayafter.Opacity = 100;
                        cloud___Copy__2__dayafter.Opacity = 100;
                        lightning_dayafter.Opacity = 100;
                        drops_dayafter.Opacity = 100;
                    }

                }
                if ((conditions.Contains("shower") || conditions.Contains("Shower") ||
                     conditions.Contains("drizzle") || conditions.Contains("Drizzle") || 
                     conditions.Contains("outbreak") || conditions.Contains("Outbreak")))
                {
                    if (day == 0)
                    {
                        cloud.Opacity = 50;
                        cloud___Copy__2_.Opacity = 100;                    
                        raining.Begin();
                        raining.RepeatBehavior = RepeatBehavior.Forever;
                    }
                    else if (day == 1)
                    {
                        cloud_today.Opacity = 100;
                        cloud___Copy__2__today.Opacity = 100;
                        drops_today.Opacity = 100;
                    }
                    else if (day == 2)
                    {
                        cloud_tomm.Opacity = 100;
                        cloud___Copy__2__tomm.Opacity = 100;
                        drops_tomm.Opacity = 100;
                    }
                    else if (day == 3)
                    {
                        cloud_dayafter.Opacity = 100;
                        cloud___Copy__2__dayafter.Opacity = 100;
                        drops_dayafter.Opacity = 100;
                    }

                }
            else if (conditions.Contains("Overcast") || (conditions.Contains("overcast") ||
                    (conditions.Contains("Mist") || (conditions.Contains("mist")))))
            {
                if (day == 0)
                {
                    cloud.Opacity = 100;
                }
                else if (day == 1)
                {
                    cloud_today.Opacity = 100;
                }
                else if (day == 2)
                {
                    cloud_tomm.Opacity = 100;
                }
                else if (day == 3)
                {
                    cloud_dayafter.Opacity = 100;
                }
            }
            else if (conditions.Contains("Sun") || (conditions.Contains("sun") ||
              (conditions.Contains("clear") || (conditions.Contains("Clear")))))//FOR CLEAR CHECK OBSERVATION TIME
            {   
                                                                //NO SUN AT NIGHT FOOL! 
                if (day == 0)
                {
                    Sun.Opacity = 100;
                    sunGlow.Begin();
                    sunGlow.RepeatBehavior = RepeatBehavior.Forever;
                    cloud.Opacity = 30;
                }
                else if (day == 1)
                {
                    Sun_today.Opacity = 100;
                    cloud_today.Opacity = 30;
                }
                else if (day == 2)
                {
                    Sun_tomm.Opacity = 100;
                    cloud_tomm.Opacity = 30;
                }
                else if (day == 3)
                {
                    Sun_dayafter.Opacity = 100;
                    cloud_dayafter.Opacity = 30;
                }
            }
                if (conditions.Contains("snow") || conditions.Contains("Snow") ||
                   conditions.Contains("Sleet") || conditions.Contains("sleet") ||
                    conditions.Contains("blizzard") || conditions.Contains("Blizzard"))
                {
                    if (day == 0)
                    {
                        cloud.Opacity = 100;
                        cloud___Copy__2_.Opacity = 100;
                        snowing.Begin();
                        snowing.RepeatBehavior = RepeatBehavior.Forever;
                    }
                    else if (day == 1)
                    {
                        cloud_today.Opacity = cloud___Copy__2__today.Opacity = 100;
                        snow_today.Opacity = 100;
                    }
                    else if (day == 2)
                    {
                        cloud_tomm.Opacity = cloud___Copy__2__tomm.Opacity = 100;
                        snow_tomm.Opacity = 100;
                    }
                    else if (day == 3)
                    {
                        cloud_dayafter.Opacity = cloud___Copy__2__dayafter.Opacity = 100;
                        snow_day_after.Opacity = 100;
                    }
                }
                if (time == null)
                {
                    getTime();
                }
                else
                {
                    int x = Int32.Parse(time.Substring(0, 2));
                    if(conditions!=null && conditions.Contains("Clear"))
                    {
                        if ((x >= 19) || (x <= 6) && (conditions.Contains("Clear")))
                        {
                            moonlayer.Opacity = 100;
                            cloud.Opacity = 10;
                            Sun.Opacity = 0;
                        }
                    }

                }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SlideTransition transition = new SlideTransition();
            transition.Mode = SlideTransitionMode.SlideRightFadeIn;
            PhoneApplicationPage phonePage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;

            ITransition trans = transition.GetTransition(phonePage);
            trans.Completed += delegate { trans.Stop(); };
            trans.Begin();      


            NavigationContext.QueryString.TryGetValue("myparameter1", out location);
            NavigationContext.QueryString.TryGetValue("myparameter2", out tempType);
            NavigationContext.QueryString.TryGetValue("myparameter3", out time);
            if(IsolatedStorageSettings.ApplicationSettings.Contains("curr"))
            {
              if (IsolatedStorageSettings.ApplicationSettings["curr"] != null)
              {
                   location = IsolatedStorageSettings.ApplicationSettings["curr"].ToString();
               }
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("tempType"))
            {
                if (IsolatedStorageSettings.ApplicationSettings["tempType"] != null)
                {
                    tempType = IsolatedStorageSettings.ApplicationSettings["tempType"].ToString();
                }
            }
            if (location!=null)
            {
     
                    getTime();
    
              
                if (location != "")
                {
                    if (location.Contains(" "))
                    {
                        int x = location.IndexOf(" ");
                        location = char.ToUpper(location[0]) + location.Substring(1, x) + char.ToUpper(location[x + 1]) + location.Substring(x + 2);
                        
                    }
                    else
                    {
                        location = char.ToUpper(location[0]) + location.Substring(1);
                    }

                    textBlock1.Text = location;
                }
               

                WebClient webclient = new WebClient();
                webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webclient_DownloadStringCompleted);

                webclient.DownloadStringAsync(new Uri("http://free.worldweatheronline.com/feed/weather.ashx?key=bed6524371124406111310&q=" + location + "&num_of_days=3&format=xml")); // weather location 
                //http://free.worldweatheronline.com/feed/weather.ashx?key=bed6524371124406111310&q=mumbai,india&num_of_days=3&format=xml
            }
        }


        string getTemp(string xmlForecast, int day)        // MAKE THIS A FUNCTION!!!!
        {
            int startFore, endFore;
            string t_celsius2, t_fahren2, conditions2;
            if (tempType == "celsius")
            {              
                startFore = xmlForecast.IndexOf("<tempMinC>") + 10;  //getting min temp C
                endFore = xmlForecast.IndexOf("</tempMinC>");
                t_celsius2 = xmlForecast.Substring(startFore, endFore - startFore) + "° - ";
                startFore = xmlForecast.IndexOf("<tempMaxC>") + 10;  //getting max temp C
                endFore = xmlForecast.IndexOf("</tempMaxC>");
                t_celsius2 += xmlForecast.Substring(startFore, endFore - startFore);
           
                 if (day == 1)
                     textBlock5.Text = t_celsius2 + "°";
                else if(day==2)
                     textBlock8.Text = t_celsius2 + "°";
                 else if(day==3)
                     textBlock11.Text = t_celsius2 + "°";
            
            }
            else if (tempType == "fahrenheit")
            {
                startFore = xmlForecast.IndexOf("<tempMinF>") + 10;  //getting min temp C
                endFore = xmlForecast.IndexOf("</tempMinF>");
                t_fahren2 = xmlForecast.Substring(startFore, endFore - startFore) + "° - ";
                startFore = xmlForecast.IndexOf("<tempMaxF>") + 10;  //getting max temp C
                endFore = xmlForecast.IndexOf("</tempMaxF>");
                t_fahren2 += xmlForecast.Substring(startFore, endFore - startFore);
                textBlock5.Text = t_fahren2 + "°";
                if (day == 1)
                    textBlock5.Text = t_fahren2 + "°";
                else if (day == 2)
                    textBlock8.Text = t_fahren2 + "°";
                else if (day == 3)
                    textBlock11.Text = t_fahren2 + "°";
            }

           
            startFore = xmlForecast.IndexOf("<weatherDesc>") + 22; //get conditions
            endFore = xmlForecast.IndexOf("</weatherDesc>");
            conditions2 = xmlForecast.Substring(startFore, endFore - (startFore + 3));
            if (day == 1)
                textBlock4.Text = conditions2;
            else if (day == 2)
                textBlock7.Text = conditions2;
            else if (day == 3)
                textBlock10.Text = conditions2;
            
            return conditions2;
        }

          void webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Network Error");

            }
            else
            {
              string xmlForecast;
              string t_celsius, t_fahren, conditions;
               int startTemp_C,endTemp_C,startTemp_F,endTemp_F,startCond,endCond,startFore;
           
              XElement XmlWeather = XElement.Parse(e.Result);
              string xmlcode=XmlWeather.ToString();
         
              if (xmlcode.Contains("error"))
              {
                  MessageBox.Show("Invalid Location");
              }

              else
              {
                  if (tempType == "celsius")
                  {
                      startTemp_C = xmlcode.IndexOf("temp_C>") + 7;
                      endTemp_C = xmlcode.IndexOf("</temp_C");
                      t_celsius = xmlcode.Substring(startTemp_C, endTemp_C - startTemp_C);
                      textBlock.Text = t_celsius + "°";
                  }

                  if (tempType == "fahrenheit")
                  {
                      startTemp_F = xmlcode.IndexOf("temp_F>") + 7;
                      endTemp_F = xmlcode.IndexOf("</temp_F");
                      t_fahren = xmlcode.Substring(startTemp_F, endTemp_F - startTemp_F);
                      textBlock.Text = t_fahren + "°";
                  }

                  startCond = xmlcode.IndexOf("<weatherDesc>") + 22;
                  endCond = xmlcode.IndexOf("</weatherDesc>");
                  conditions = xmlcode.Substring(startCond, endCond - (startCond + 3));
                  textBlock2.Text = conditions;
                  condition_day0 = conditions;
                  showConditions(conditions, 0);


                  //1ST DAY FORECAST EXTRACTION


                  startFore = xmlcode.IndexOf("<tempMaxC>");   //cutting code to get forecast
                  xmlForecast = xmlcode.Substring(startFore, xmlcode.Length - startFore);

                  string cond=getTemp(xmlForecast,1);

                  showConditions(cond, 1);

                  //2ND DAY FORECAST EXTRACTION


                  startFore = xmlForecast.IndexOf("</weatherDesc>") + 14;   //cutting code to get 2nd day forecast
                  xmlForecast = xmlForecast.Substring(startFore, xmlForecast.Length - startFore);

                  cond=getTemp(xmlForecast, 2);

                  showConditions(cond, 2);

                  //3RD DAY FORECAST EXTRACTION


                  startFore = xmlForecast.IndexOf("</weatherDesc>") + 14;   //cutting code to get 2nd day forecast
                  xmlForecast = xmlForecast.Substring(startFore, xmlForecast.Length - startFore);

                  cond = getTemp(xmlForecast, 3);

                  showConditions(cond, 3);

                  
              }
              }
                
              
        }

        
       
         

         
         
        private void Layer_1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            Page1 p = new Page1();
            NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));

           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
        }
       

       

        
    }
    }
        
    