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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace UITrial
{
    public partial class Page1 : PhoneApplicationPage
    {
        private string periodicTaskName = "ClimaticaLiveTilePeriodicAgent";
        private PeriodicTask periodicTask;
       
        public Page1()
        {
            InitializeComponent();
        }

       
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

          //  base.OnNavigatedTo(e);
            SlideTransition transition = new SlideTransition();
            transition.Mode = SlideTransitionMode.SlideRightFadeIn;
            PhoneApplicationPage phonePage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;

            ITransition trans = transition.GetTransition(phonePage);
            trans.Completed += delegate { trans.Stop(); };
            trans.Begin();           
 

                if (IsolatedStorageSettings.ApplicationSettings.Contains("curr"))
                {
                    textBox1.Text = IsolatedStorageSettings.ApplicationSettings["curr"].ToString();                 
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("tempType") &&
                       IsolatedStorageSettings.ApplicationSettings["tempType"] != null)
                {
                    if (IsolatedStorageSettings.ApplicationSettings["tempType"].ToString() == "celsius")                   
                        radioButton1.IsChecked = true;
                    
                    else                   
                        radioButton2.IsChecked = true;
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("livetile") &&
                           IsolatedStorageSettings.ApplicationSettings["livetile"] != null)
                {
                    if (IsolatedStorageSettings.ApplicationSettings["livetile"].ToString() == "on")
                        toggle1.IsChecked = true;

                    else
                        toggle1.IsChecked = false;
                }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string parameter1Value = textBox1.Text;
            string parameter2Value = "'ello";
            if (radioButton1.IsChecked == true)
                parameter2Value = "celsius";
            else
                parameter2Value = "fahrenheit";
           
            IsolatedStorageSettings.ApplicationSettings.Remove("tempType");         
            IsolatedStorageSettings.ApplicationSettings.Remove("curr");                                   
            IsolatedStorageSettings.ApplicationSettings.Add("curr", textBox1.Text);                                          
            IsolatedStorageSettings.ApplicationSettings.Add("tempType",parameter2Value);
            IsolatedStorageSettings.ApplicationSettings.Save();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("livetile") &&
                          IsolatedStorageSettings.ApplicationSettings["livetile"] != null)
            {
                if (IsolatedStorageSettings.ApplicationSettings["livetile"].ToString() == "on")
                {
                    instantUpdate();
                    StartPeriodicAgent();
                }

                else
                {
                    
                /*    ShellTile Tile = ShellTile.ActiveTiles.First();
                    StandardTileData data = new StandardTileData();
                    data.BackgroundImage = new Uri("/Images/Background.png", UriKind.Relative);
                    data.Title = "Climatica";
                    data.Count = null;
                    data.BackBackgroundImage = new Uri("",UriKind.Relative);
                    data.BackContent = string.Empty;
                    data.BackTitle = string.Empty;
                    Tile.Update(data);
                  //  NotifyComplete();
                    StopPeriodicAgent();     */
                    
                }
            }
            
            
           
            NavigationService.Navigate(new Uri(string.Format("/MainPage.xaml?myparameter1={0}&myparameter2={1}",
        parameter1Value, parameter2Value),
        UriKind.Relative));

           
        }

        string conditions2, time, conditionsTomm;
        int startTemp_C, endTemp_C, startTemp_F, endTemp_F;
        string temperature, t_celsius2;
        string location = "";

        void instantUpdate()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("curr"))
            {
                location = IsolatedStorageSettings.ApplicationSettings["curr"].ToString();
            }
            // some random number

            // get application tile
            ShellTile tile = ShellTile.ActiveTiles.First();

            if (null != tile)
            {
                WebClient webclient = new WebClient();
                webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted2);

                webclient.DownloadStringAsync(new Uri("http://free.worldweatheronline.com/feed/weather.ashx?key=bed6524371124406111310&q=" + location + "&num_of_days=3&format=xml")); // weather location 
                //http://free.worldweatheronline.com/feed/weather.ashx?key=bed6524371124406111310&q=mumbai,india&num_of_days=3&format=xml
            }


        }

        void client_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Network Error");

            }
            else
            {
                int startFore, endFore;

                XElement xmlForecast1 = XElement.Parse(e.Result);
                string xmlForecast = xmlForecast1.ToString();
                if (xmlForecast.Contains("error"))
                {
                    //       MessageBox.Show("Invalid Location");
                }
                else
                {
                    //GET THE LOCAL TIME
                    {
                        /*  int start, end;
                          start = xmlForecast.IndexOf("<localtime>") + 11 + 11;//extra 11 is to avoid the date in xml
                          end = xmlForecast.IndexOf("</localtime>");
                          time = xmlForecast.Substring(start, end - start);*/
                    }

                    if (IsolatedStorageSettings.ApplicationSettings.Contains("tempType") &&
                       IsolatedStorageSettings.ApplicationSettings["tempType"] != null)
                    {
                        if (IsolatedStorageSettings.ApplicationSettings["tempType"].ToString() == "celsius")
                        //GET TEMPERATURE CELSIUS
                        {
                            startTemp_C = xmlForecast.IndexOf("temp_C>") + 7;
                            endTemp_C = xmlForecast.IndexOf("</temp_C");
                            temperature = xmlForecast.Substring(startTemp_C, endTemp_C - startTemp_C) + "°C";
                        }
                        else
                        //GET TEMPERATURE FAHRENHEIT 
                        {
                            startTemp_F = xmlForecast.IndexOf("temp_F>") + 7;
                            endTemp_F = xmlForecast.IndexOf("</temp_F");
                            temperature = xmlForecast.Substring(startTemp_F, endTemp_F - startTemp_F) + "°F";
                        }
                    }
                    //GET CONDITIONS
                    {
                        startFore = xmlForecast.IndexOf("<weatherDesc>") + 22; //get conditions
                        endFore = xmlForecast.IndexOf("</weatherDesc>");
                        conditions2 = xmlForecast.Substring(startFore, endFore - (startFore + 3));

                        startFore = xmlForecast.IndexOf("<tempMaxC>");   //cutting code to get forecast
                        string xmlForecastCond = xmlForecast.Substring(startFore, xmlForecast.Length - startFore);//Using ForeCond so as to not disturb xmlForecast for further usage!!!
                        startFore = xmlForecastCond.IndexOf("<weatherDesc>") + 22; //get conditions
                        endFore = xmlForecastCond.IndexOf("</weatherDesc>");

                        startFore = xmlForecastCond.IndexOf("</weatherDesc>") + 14;   //cutting code to get 2nd day forecast
                        xmlForecastCond = xmlForecastCond.Substring(startFore, xmlForecastCond.Length - startFore);

                        if (IsolatedStorageSettings.ApplicationSettings.Contains("tempType") &&
                      IsolatedStorageSettings.ApplicationSettings["tempType"] != null)
                        {
                            if (IsolatedStorageSettings.ApplicationSettings["tempType"].ToString() == "celsius")
                            {
                                startFore = xmlForecastCond.IndexOf("<tempMinC>") + 10;  //getting min temp C
                                endFore = xmlForecastCond.IndexOf("</tempMinC>");
                                t_celsius2 = xmlForecastCond.Substring(startFore, endFore - startFore) + "°C - ";
                                startFore = xmlForecastCond.IndexOf("<tempMaxC>") + 10;  //getting max temp C
                                endFore = xmlForecastCond.IndexOf("</tempMaxC>");
                                t_celsius2 += xmlForecastCond.Substring(startFore, endFore - startFore) + "°C";
                            }

                            else
                            {
                                startFore = xmlForecastCond.IndexOf("<tempMinF>") + 10;  //getting min temp C
                                endFore = xmlForecastCond.IndexOf("</tempMinF>");
                                t_celsius2 = xmlForecastCond.Substring(startFore, endFore - startFore) + "°F - ";
                                startFore = xmlForecastCond.IndexOf("<tempMaxF>") + 10;  //getting max temp C
                                endFore = xmlForecastCond.IndexOf("</tempMaxF>");
                                t_celsius2 += xmlForecastCond.Substring(startFore, endFore - startFore) + "°F";
                            }
                        }

                        startFore = xmlForecastCond.IndexOf("</tempMaxC>") + 14;   //cutting code to get 2nd day forecast
                        xmlForecastCond = xmlForecastCond.Substring(startFore, xmlForecastCond.Length - startFore);
                        startFore = xmlForecastCond.IndexOf("<weatherDesc>") + 22; //get conditions
                        endFore = xmlForecastCond.IndexOf("</weatherDesc>");
                        conditionsTomm = xmlForecastCond.Substring(startFore, endFore - (startFore + 3));


                    }
                    StandardTileData data = new StandardTileData();
                    // tile foreground data
                    ShellTile tile = ShellTile.ActiveTiles.First();
                    if (location.Contains(" "))
                    {
                        int x = location.IndexOf(" ");
                        location = char.ToUpper(location[0]) + location.Substring(1, x) + char.ToUpper(location[x + 1]) + location.Substring(x + 2);
                    }
                    else
                    {
                        location = char.ToUpper(location[0]) + location.Substring(1);
                    }
                    data.Title = location + ",  " + temperature;// conditions2;
                    if (conditions2 != null)
                    {
                        if (conditions2.Contains("cloud") || conditions2.Contains("Cloud") || (conditions2.Contains("Overcast") ||
                            (conditions2.Contains("overcast") || (conditions2.Contains("Mist") || (conditions2.Contains("mist"))))))
                        {
                            data.BackgroundImage = new Uri("/Images/cloudtile2.png", UriKind.Relative);
                        }
                        else if (conditions2.Contains("Clear") || conditions2.Contains("clear"))
                        {
                            data.BackgroundImage = new Uri("/Images/moontile.png", UriKind.Relative);
                        }
                        else if (conditions2.Contains("sun") || conditions2.Contains("Sun"))
                        {
                            data.BackgroundImage = new Uri("/Images/suntile.png", UriKind.Relative);
                        }
                        else if (conditions2.Contains("rain") || conditions2.Contains("Rain") || conditions2.Contains("shower") ||
                            conditions2.Contains("Shower") || conditions2.Contains("drizzle") || conditions2.Contains("Drizzle") ||
                     conditions2.Contains("outbreak") || conditions2.Contains("Outbreak"))
                        {
                            data.BackgroundImage = new Uri("/Images/raintile.png", UriKind.Relative);
                        }
                        else if (conditions2.Contains("snow") || conditions2.Contains("Snow") ||
                  conditions2.Contains("Sleet") || conditions2.Contains("sleet") ||
                   conditions2.Contains("blizzard") || conditions2.Contains("Blizzard"))
                        {
                            data.BackgroundImage = new Uri("/Images/snowtile.png", UriKind.Relative);
                        }
                    }
                    Random random = new Random();
                //    data.Count = random.Next(100);

                    data.BackTitle = t_celsius2;
                    data.BackBackgroundImage = new Uri("/Images/Green.jpg", UriKind.Relative);
                    data.BackContent = "Tomorrow: " + conditionsTomm;
                    tile.Update(data);
                   // NotifyComplete();
                }
            }
        }



        private void toggle1_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("livetile");
            IsolatedStorageSettings.ApplicationSettings.Add("livetile", "on");
            IsolatedStorageSettings.ApplicationSettings.Save();
            toggle1.Content = "On";
            textblockInfo.Text = "Switch off Live Tile Update if you would like to stop receiving live weather updates";
            instantUpdate();
            StartPeriodicAgent();
        }

        private void toggle1_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("livetile");
            IsolatedStorageSettings.ApplicationSettings.Add("livetile", "off");
            IsolatedStorageSettings.ApplicationSettings.Save();
            toggle1.Content = "Off";

            textblockInfo.Text = "To receive live weather updates, make sure the Climatica Icon is pinned to your Start Screen and switch on the Live Tile Update";
            ShellTile Tile = ShellTile.ActiveTiles.First();
            StandardTileData data = new StandardTileData();
            data.BackgroundImage = new Uri("/Images/Background.png", UriKind.Relative);
            data.Title = "Climatica";
      //      data.Count = null;
            data.BackBackgroundImage = new Uri("", UriKind.Relative);
            data.BackContent = string.Empty;
            data.BackTitle = string.Empty;
            Tile.Update(data);
            //  NotifyComplete();
            StopPeriodicAgent();
        }

        private void StopPeriodicAgent()
        {
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(periodicTaskName);
                }
                catch (Exception)
                {
                }
            }
        }

        private void StartPeriodicAgent()
        {
            // is old task running, remove it
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(periodicTaskName);
                }
                catch (Exception)
                {
                }
            }
            // create a new task
            periodicTask = new PeriodicTask(periodicTaskName);
            // load description from localized strings
            periodicTask.Description = "This is LiveTile application update agent.";
            // set expiration days
            periodicTask.ExpirationTime = DateTime.Now.AddDays(14);
            try
            {
                // add thas to scheduled action service
                ScheduledActionService.Add(periodicTask);
                // debug, so run in every 30 secs

#if(DEBUG_AGENT)            
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
                System.Diagnostics.Debug.WriteLine("Periodic task is started: " + periodicTaskName);
#endif

            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    // load error text from localized strings
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
        }

    }
}