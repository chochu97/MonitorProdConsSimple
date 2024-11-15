using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsMonitor
{
    internal class Program // Ejercicio Productor-Consumidor, Monitor y Threads
    {
        private static Queue<string> productionQueue = new Queue<string>();  // La Queue/Buffer para el patron Productor-Consumidor
        private static readonly object _lock = new object(); // el objeto _lock de bloqueo para la sincronizacion
        private const int bufferSize = 10;  // En este caso pide que la Queue tenga un size fijo/maximo para no sobresaturar

        static void Main(string[] args)
        {

            Thread productor1 = new Thread(() => Produce("Maquina 1"));
            Thread productor2 = new Thread(() => Produce("Maquina 5"));

            Thread consumidor1 = new Thread(() => Consume("Equipo Ensamblaje 3"));
            Thread consumidor2 = new Thread(() => Consume("Equipo Ensamblaje 6"));

            productor1.Start();
            productor2.Start();
            consumidor1.Start();
            consumidor2.Start();

            productor1.Join();
            productor2.Join();
            
            consumidor1.Join();
            consumidor2.Join();

            Console.WriteLine("Produccion y Ensamblado completado.");
        }

        static void Produce(string machineName)
        {
            for(int i = 1; i <= 15; i++) // tiramos un numero de que cada maquina produce unos 15 componentes electronicos
            {
                lock (_lock)
                {
                    while(productionQueue.Count >= bufferSize)
                    {
                        Monitor.Wait(_lock);
                    }

                    string component = $"{machineName} - Componente Electronico {i}";  // creamos el string acorde al componente y la maquina que lo crea
                    productionQueue.Enqueue(component); // lo agregamos a la cola
                    Console.WriteLine($"Produccion finalizada: {component}");

                    Monitor.Pulse(_lock); // Notifica a un consumidor que hay un nuevo componente en la queue

                }
                Thread.Sleep(new Random().Next(100, 500)); // simulamos un tiempo de produccion
            }
        }

        static void Consume(string AssembleTeamName)
        {
            while (true)
            {
                string component;
                lock (_lock)
                {
                    while(productionQueue.Count == 0)  // Si la cola esta vacia, espera.
                    {
                        Monitor.Wait(_lock);
                    }
                     
                    component = productionQueue.Dequeue();  // toma un componente del queue (lo meto en una variable al sacarlo)
                    Console.WriteLine($"{AssembleTeamName} ensamblo: {component}");

                    Monitor.Pulse(_lock); // notifica al productor que hay un espacio libre en la queue
                }
                Thread.Sleep(new Random().Next(300, 600));
            }
        }
    }
}
