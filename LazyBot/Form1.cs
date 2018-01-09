using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIMLbot;
using Telegram;
using Telegram.Bot.Types;

namespace LazyBot
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        public Form1()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.DoWork += do_work;
        }

        async void do_work(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String;
            try
            {
                var bot = new Telegram.Bot.TelegramBotClient(key);
                await bot.SetWebhookAsync("");
                AIMLbot.Bot lazy = new AIMLbot.Bot();


                lazy.loadSettings();
                lazy.loadAIMLFromFiles();
                lazy.isAcceptingUserInput = false;
                AIMLbot.User us = new AIMLbot.User("Username", lazy);
                lazy.isAcceptingUserInput = true;


                bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    Telegram.Bot.Types.FileToSend s;
                    var message = ev.CallbackQuery.Message;
                    if (ev.CallbackQuery.Data == "callback1")
                    {
                        s = new Telegram.Bot.Types.FileToSend("https://i.pinimg.com/originals/f7/e9/80/f7e980c9700c8395535b835e66f02a59.jpg");
                    }
                    else if (ev.CallbackQuery.Data == "callback2")
                    {
                        s = new Telegram.Bot.Types.FileToSend("https://static.independent.co.uk/s3fs-public/thumbnails/image/2012/02/29/22/pg-28-sloth-cooke.jpg");
                    }
                    await bot.SendPhotoAsync(message.Chat.Id, s, "Sure! But... not today :)");
                    await bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                };

                bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; 
                    var update = evu.Update;
                    var message = update.Message;
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        if (message.Text[0] == '/')
                        {
                            if (message.Text == "/test")
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "Yeah...",
                                    replyToMessageId: message.MessageId);
                            }

                            if (message.Text == "/sleep")
                            {
                                var s = new Telegram.Bot.Types.FileToSend("https://beano-uploads-production.imgix.net/store/f4046f22dffe92e3f2167accb6942f788159d0f979f970dcda59f1d0e529?auto=compress&w=752&h=423&fit=min");
                                await bot.SendPhotoAsync(message.Chat.Id,
                                    s, "Yeeeeeeeeeah, sleeeeeeeeep!");
                            }

                            if (message.Text == "/song")
                            {
                                var s = new Telegram.Bot.Types.FileToSend("http://store.naitimp3.ru/download/0/cGR1a0tRTWJwZW8wMlI2aitkT1UzVkxNdXE2dUNiRTAvcGRkeGphMTVFVTdQcGFURWlFOFQyUGZFTXJ6UVo4cWxVSUNza2NOQUpoZkJOU2ozYTJhWUpLSVdiUTRTanQrVmZnN1hQV1U5Tkk9/eels_i_need_some_sleep_(NaitiMP3.ru).mp3");
                                await bot.SendAudioAsync(message.Chat.Id, s, "", 4, "Eels", "I need some sleep...");
                            }

                            if (message.Text == "/work")
                            {
                                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                                                        new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][]
                                                        {
                                                            // First row
                                                            new []{
                                                                // First column
                                                                new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Work", "callback1"),

                                                                // Second column
                                                                new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Work harder","callback2")
                                                            },
                                                        }
                                                    );

                                await bot.SendTextMessageAsync(message.Chat.Id, "Hmmmm... What should I do?..!", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, keyboard);
                            }

                        }
                        else if ((message.Text[0] >= 'а' && message.Text[0] <= 'я') || (message.Text[0] >= 'А' && message.Text[0] <= 'Я'))
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "I'm too lazy to learn russian, sorry...",
                                    replyToMessageId: message.MessageId);
                        }
                        else
                        {

                            AIMLbot.Request r = new AIMLbot.Request(message.Text, us, lazy);
                            AIMLbot.Result res = lazy.Chat(r);

                            await bot.SendTextMessageAsync(message.Chat.Id, res.Output,
                                replyToMessageId: message.MessageId);
                        }
                    }
                };
                
                bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text;

            if (text != "" && bw.IsBusy != true)
            {
                bw.RunWorkerAsync(text);
            }

            MessageBox.Show("Done!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}