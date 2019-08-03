using System;
using System.IO;
using System.Threading;
using Mogster.Core.Harness.FFXIVPlugin;
using FFXIV_ACT_Plugin.Logfile;
namespace Mogster.Replay
{

    public class Replay : IDisposable
    {
        private Timer timer;
        private StreamReader logReader;
        private string logPath;
        private double msTillNextMessage;
        private string nextMessage;
        private LogMessageType nextMessageType;
        private DateTime nextMessageTime;
        private bool disposed = false;
        private BlankLogOutput logOutput;
        private double timeFactor = 1.0;
        private bool paused = false;


        public double TimeFactor
        {
            get
            {
                return timeFactor;
            }
            set
            {
                msTillNextMessage = msTillNextMessage / timeFactor * value; //adjust remaining time till next message to use new timefactor
                timeFactor = value;
            }
        }



        public Replay(string logFilePath, BlankLogOutput blankLogOutput)
        {
            //setup reader
            logPath = logFilePath;
            logOutput = blankLogOutput;
            timer = new Timer(TimePulse, null, Timeout.Infinite, Timeout.Infinite); //create it, but don't set it running yet.
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //close the logReader if its open.
                logReader?.Close();
                logReader?.Dispose();
                logReader = null;

                //pause the timer and call its dispose.
                Pause(); 
                timer.Dispose();
            }
        }

        private void TimePulse(object state)
        {
            
            
            if (msTillNextMessage > 0)
            {
                msTillNextMessage -= 1;
                return;
            }

            //work to do, pause timer, parse messages then resume
            Pause();

            do
            {
                logOutput.WriteLine(nextMessageType, nextMessageTime, nextMessage);

                //store the old message as current
                var currentMessage = nextMessage;
                var currentMessageTime = nextMessageTime;
                var currentMessageType = nextMessageType;

                //grab the next line, if not next line pause timer and break.
                if(!ParseNextMessage())
                {
                    break; //I'm jsut stopping after I hit the last line, need a way to inform the caller than replay has finished.
                }

                //calculate next message then adjust by timefactor
                msTillNextMessage = (nextMessageTime - currentMessageTime).TotalMilliseconds * TimeFactor; 

            } while (msTillNextMessage < 1.0); // the next event happened in the same MS, loop again until we get something that happens atleast 1 ms later
            Resume();
        }

        //starts the replay, if called again resets the replay.
        public void Start()//replay already running pause it
        {
            //pause timer if its running
            Pause();


            //reset release old logReader if it exists
            logReader?.Close();
            logReader?.Dispose();

            //create new logReader
            logReader = new StreamReader(logPath);


            //reset msTillNext message and get the first line
            msTillNextMessage = 0;
            ParseNextMessage();

            //start the timer
            Resume();
        }

        //Call to pause play back
        public void Pause()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite); //sets the timer intervale to infinite
        }

        //Call to Resume after a pause
        public void Resume()
        {
            timer.Change(1, 1);
        }

        //Stops playback and releases the file
        public void Stop ()
        {
            Pause();
        }

        //returns true if a line is parsed. False if there was no more lines.
        private bool ParseNextMessage()
        {
            if (logReader.EndOfStream)
            {
                return false;
            }

            //get next line
            var logline = logReader.ReadLine();


            //split line
            string[] tokens = logline.Split(new char['|'], 3, StringSplitOptions.None);

            //store output tokens
            nextMessageType = (LogMessageType)(int.Parse(tokens[0]));
            nextMessageTime = DateTime.Parse(tokens[1]);
            nextMessage = tokens[2];

            return true;
        }
    }
}
