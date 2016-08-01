namespace MailSync
{
    using System;
    using System.Text;
    using System.Threading;
    using AE.Net.Mail;

    class Program
    {
        static void Main(string[] args)
        {
            using (var imap = new ImapClient("imap.163.com", "konger12ban@163.com", "abc12345"))
            {
                //var msgs = imap.SearchMessages(
                //  SearchCondition.Undeleted().And(
                //    SearchCondition.From("david"),
                //    SearchCondition.SentSince(new DateTime(2000, 1, 1))
                //  ).Or(SearchCondition.To("andy"))
                //);

                //Assert.AreEqual(msgs[0].Value.Subject, "This is cool!");

                //imap.NewMessage += (sender, e) => {
                //    var msg = imap.GetMessage(e.MessageCount - 1);
                //    Assert.AreEqual(msg.Subject, "IDLE support?  Yes, please!");
                //};

                imap.Encoding = Encoding.GetEncoding("gb2312");
                imap.SelectMailbox("INBOX");
                var messages = imap.GetMessages(0, 10);
                var message = messages[0];

                Console.OutputEncoding = Encoding.Unicode;
                Console.WriteLine(message.Subject);
                Console.WriteLine(message.From.DisplayName);
                Console.WriteLine(message.Date);
                Console.WriteLine(message.Body);

                Thread.Sleep(5000);


            }


        }
    }
}
