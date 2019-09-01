namespace Core.Infrastructure.MassTransit
{
    public class BusConfiguration
    {

        /// <summary>
        /// the full Uri/Url of the broker
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// the username to use with this application
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// guess what, the password for that user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// the endpoint name, use the name of the application in order to make senese.
        /// </summary>
        public string ReceiveEndpoint { get; set; }
    }
}