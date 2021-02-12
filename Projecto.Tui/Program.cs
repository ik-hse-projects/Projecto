using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    class Program
    {
        static void Main(string[] args)
        {
            var project = new Project();
            new MainLoop(new Label("Hello world!")).Start();
        }
    }
}