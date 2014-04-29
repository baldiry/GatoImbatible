using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
namespace GatoConsola
{
    class Gato
    {
        private Boolean humanoTiras;
        private Queue colaHumano = new Queue();
        private Queue colaMaquina = new Queue();
        private int?[] tablero;
        private int cantidadTiros = 0;
        LeerArchivo leer = new LeerArchivo();
        private List<String> archivo;
        private Base[] baseConocimientos;
        Motor mot = new Motor();    

        SortedDictionary<String, int> maximos; 
        SortedDictionary<String, int> minimos; 
        int niv; 
        Random rnd = new Random(); 

  
        public void guardar() //Metodo que guarda las listas.
        {
            StreamWriter sw = new StreamWriter("Minimos.txt");
            foreach (String edo in minimos.Keys) 
            {
                sw.WriteLine(edo + ":" + minimos[edo]);
            }
            sw.Flush();
            sw.Close(); 
            sw = new StreamWriter("Maximos.txt");
            foreach (String edo in maximos.Keys)
            {
                sw.WriteLine(edo + ":" + maximos[edo]);
            }
            sw.Flush();
            sw.Close();
        }
    
        public int Tirar(Motor g) // metodo para tirar de la mejor manera
        {
            int t;
            if (!g.juegoEnCurso()) return -1;
            if (g.esTurnoO()) 
            {
                int f = 0, c = 0; 
                int v = int.MaxValue; 
                int aux;
                
                for (int i = 0; i < 3; i++) 
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i)) 
                        {
                            g.tirar(j, i, 'O'); 
                         
                            
                            aux = aplicarMaximo(g, 2); 
                            if (aux < v) 
                            {
                                v = aux; 
                                f = i; 
                                c = j;
                            }
                            g.borrar(j, i); 
                           
                        }
                    }
                }
                g.tirar(c, f, 'O'); 
           
                t = (f * 3) + c;
                return t;
            }
            else 
            {
                int f = 0, c = 0;
                int v = int.MinValue; 
                int aux;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X'); 
                          
                            
                            aux = aplicarMinimo(g, 2); 
                            if (aux > v) 
                            {
                                v = aux;
                                f = i;
                                c = j;
                            }
                            g.borrar(j, i);

                          
                        }
                    }
                }
                g.tirar(c, f, 'X'); 
             
                t = (f * 3) + c;
                return t;
            }
        }

        private int aplicarMinimo(Motor g, int niv) 
        {
            if (!g.juegoEnCurso() || niv == 0) 
            {
                return g.getPuntos(); 
            }
            else
            {
                int v = int.MaxValue; 
                int aux;
                for (int i = 0; i < 3; i++) 
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i)) 
                        {
                            g.tirar(j, i, 'O');
                            aux = aplicarMaximo(g, niv - 1); 
                            if (aux<v) v = aux; 
                            g.borrar(j, i);
                        }
                    }
                }
                return v; 
            }
        }

        private int aplicarMaximo(Motor g, int niv) 
        {
            if (!g.juegoEnCurso() || niv == 0)
            {
                return g.getPuntos();
            }
            else
            {
                int v = int.MinValue; 
                int aux;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (g.estaVacia(j, i))
                        {
                            g.tirar(j, i, 'X'); 
                            aux = aplicarMinimo(g, niv - 1); 
                            if (aux>v) v = aux; 
                            g.borrar(j, i);
                        }
                    }
                }
                return v;
            }
        }

        //.-------------------------------------------------------------------------------


       
        Gato(int nivel)
        {
            tablero = new int?[9];
            archivo = leer.leerFile("BASECONOCIMIENTO_GATO.TXT");

            if (archivo.Count > 0)
            {
                baseConocimientos = new Base[archivo.Count];
                for (int t = 0; t < archivo.Count; t++)
                {
                    baseConocimientos[t] = new Base();
                    baseConocimientos[t].setGano(archivo[t].Split('|')[0]);
                    baseConocimientos[t].setPerdio(archivo[t].Split('|')[1]);
                    baseConocimientos[t].setResultado(archivo[t].Split('|')[2]);
                    baseConocimientos[t].setCantidad(Convert.ToInt32(archivo[t].Split('|')[3]));
                }
            }

            niv = nivel;
            maximos = new SortedDictionary<string, int>();
            minimos = new SortedDictionary<string, int>();
            if (File.Exists("Minimos.txt")) 
            {
                String l;
                String[] datos;
                StreamReader sr = new StreamReader("Minimos.txt");
                while ((l = sr.ReadLine()) != null) 
                {
                    datos = l.Split(':');
                    if (datos.Length != 2) continue;
                    minimos.Add(datos[0], int.Parse(datos[1])); 
                }
                sr.Close();
            }
            if (File.Exists("Maximos.txt")) 
            {
                String l;
                String[] datos;
                StreamReader sr = new StreamReader("Maximos.txt");
                while ((l = sr.ReadLine()) != null)
                {
                    datos = l.Split(':');
                    if (datos.Length != 2) continue;
                    maximos.Add(datos[0], int.Parse(datos[1]));
                }
                sr.Close();
            }

        }
        //metodo boleano que decide quien tira primero
        public Boolean humanoTira()
        {
            Random rand = new Random();
            int r = rand.Next(2);
            Console.Write(r);
            if (r == 0)
            {

                return false;
            }
            else
            {
                return true;
            }
        }
        //metodo que asigna el tiro del humano en su cola de tiros. con la finalidad de irlos guardando.
        public void agregarColaHumano(int posicion)
        {
            colaHumano.Enqueue(posicion);
        }
        //metodo que asigna el tiro de la maquina en su cola de tiros. con la finalidad de irlos guardando.
        public void agregarColaMaquina(int posicion)
        {
            colaMaquina.Enqueue(posicion);
        }
        //metodo que asigna el tiro al tablero del juego actual.
        public void agregarTiroTablero(int posicion, int valor)
        {
            tablero[posicion] = valor;
        }
        //metodo que incrementa la cantidad de tiros en 1
        public void incrementarCantidadTiros()
        {
            cantidadTiros++;
        }
        //metodo que compara si la cantidad de tiros es igual o mayor a 5
        public Boolean deboComparar(int cantidad)
        {
            if (cantidadTiros >= cantidad)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //metodo que valida si se gano en el tablero.
        public Boolean seGano(int numero)
        {

            for (int t = 0; t < tablero.Length; t++)
            {
                if (tablero[0] + tablero[1] + tablero[2] == numero)
                {
                    return true;
                }
                if (tablero[3] + tablero[4] + tablero[5] == numero)
                {
                    return true;
                }
                if (tablero[6] + tablero[7] + tablero[8] == numero)
                {
                    return true;
                }
                if (tablero[0] + tablero[3] + tablero[6] == numero)
                {
                    return true;
                }
                if (tablero[1] + tablero[4] + tablero[7] == numero)
                {
                    return true;
                }
                if (tablero[2] + tablero[5] + tablero[8] == numero)
                {
                    return true;
                }
                if (tablero[0] + tablero[4] + tablero[8] == numero)
                {
                    return true;
                }
                if (tablero[6] + tablero[4] + tablero[2] == numero)
                {
                    return true;
                }

            }

            return false;
        }
        public Boolean hayTiros()
        {
            if (cantidadTiros < 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Busca el mejor tiro de la base de conocimientos basandose en quién tenga la mayor puntuación.
        public Base buscarMejorTiro()
        {
            int contadorTemp = 0;
            if (baseConocimientos.Length > 0)
            {
                int? contador = null;
                for (int t = 0; t < baseConocimientos.Length; t++)
                {

                    if (t == 0)
                    {
                        contador = baseConocimientos[t].getCantidad();
                        contadorTemp = t;
                    }
                    if (baseConocimientos[t].getCantidad() > contador)
                    {
                        contador = baseConocimientos[t].getCantidad();
                        contadorTemp = t;
                    }

                }


            }
            return baseConocimientos[contadorTemp];
        }
        //Busca el mejor tiro con la secuencia de tiros que lleva la cola de cada jugador
        public Base buscarMejorTiro(String g, String b)
        {
            int contadorTemp = 0;
            List<Base> tirosCoincidencia = new List<Base>();

            if (baseConocimientos.Length > 0)
            {
                int? contador = null;


                for (int t = 0; t < baseConocimientos.Length; t++)
                {
                    if (baseConocimientos[t].getGano().StartsWith(g) & baseConocimientos[t].getPerdio().StartsWith(b))
                    {
                        tirosCoincidencia.Add(baseConocimientos[t]);
                    }

                }

                for (int t = 0; t < tirosCoincidencia.Count; t++)
                {

                    if (t == 0)
                    {
                        contador = tirosCoincidencia[t].getCantidad();
                        contadorTemp = t;
                    }
                    if (tirosCoincidencia[t].getCantidad() > contador)
                    {
                        contador = tirosCoincidencia[t].getCantidad();
                        contadorTemp = t;
                    }

                }


            }
            if (tirosCoincidencia.Count > 0)
            {
                return tirosCoincidencia[contadorTemp];
            }
            else
            {
                return null;
            }

        }


        public int leerTiro()
        {
            ConsoleKeyInfo cki;
            Console.TreatControlCAsInput = true;

            Console.WriteLine("Indique la posicion de su tiro (1 a 9): ");
            cki = Console.ReadKey();
            if (!char.IsNumber(cki.KeyChar.ToString(), 0))
            {
                Console.WriteLine("Debe ingresar un número entre 1 y 9");
                return leerTiro();
            }
            int number = Convert.ToInt32(cki.KeyChar.ToString());

            Console.WriteLine(cki.KeyChar);
            if (number > 0 && number < 10)
                if (tablero[number - 1] == null)
                {
                    return number - 1;
                }
                else
                {
                    Console.WriteLine("El tiro ingresado ya esta ocupado porfavor tire en otra casilla.");
                    return leerTiro();
                }

            else
            {
                Console.WriteLine("Debe ingresar un número entre 1 y 9");
                return leerTiro();
            }
        }
        //pinta el tablero en la interfaz grafica.
        public void pintarTablero()
        {
            Console.Write("\nTablero:\n");
            for (int t = 0; t < tablero.Length; t++)
            {
                Console.Write("|" + (tablero[t] == null ? " " : tablero[t] + "") + "|");
                if (t == 2 | t == 5 | t == 8)
                {
                    Console.Write("|\n");
                }
            }
            Console.Write("\nTablero:");
        }

        public int tiroAzar()
        {
            for (int t = 0; t < tablero.Length; t++)
            {
                if (tablero[t] == null)
                {
                    return t;
                }
            }

            return -1;

        }

        public void jugarHumano()
        {
            pintarTablero();
            incrementarCantidadTiros();
            int tiroHuman = -1;
            tiroHuman = leerTiro();
            colaHumano.Enqueue(tiroHuman);
            tablero[tiroHuman] = 1;
            if (deboComparar(5))
            {
                if (seGano(3))
                {
                    Console.WriteLine("Se ganó con usuario");
                    pintarTablero();
                    almacenarResultado(true, "BASECONOCIMIENTO_GATO.TXT");
                }
                else
                {
                    if (hayTiros())
                    {

                        jugarMaquinaMini();
                    }
                    else
                    {
                        pintarTablero();
                    }
                }
            }
            else
            {

                jugarMaquinaMini();
            }

        }

        public void jugarHumanoMini()
        {
            pintarTablero();
            incrementarCantidadTiros();
            int tiroHuman = -1;
            tiroHuman = leerTiro();
            colaHumano.Enqueue(tiroHuman);
            int[] posicion = conviertePos(tiroHuman);
            mot.tirar(posicion[1], posicion[0],'X');
            tablero[tiroHuman] = 1;
            if (deboComparar(5))
            {
                if (seGano(3))
                {
                    Console.WriteLine("Se gano con usuario");
                    pintarTablero();
                    almacenarResultado(true, "BASECONOCIMIENTO_GATO.TXT");
                }
                else
                {
                    if (hayTiros())
                    {

                        jugarMaquinaMini();
                    }
                    else
                    {
                        pintarTablero();
                    }
                }
            }
            else
            {

                jugarMaquinaMini();
            }

        }

        public void jugarMaquina()
        {

            pintarTablero();
            incrementarCantidadTiros();
            if (baseConocimientos.Length > 0)
            {
                String tirosRival = "";
                String tirosMaquina = "";
                if (colaHumano.Count > 0)
                {
                    Object[] colHum = colaHumano.ToArray();

                    for (int t = 0; t < colHum.Length; t++)
                    {
                        tirosRival += colHum[t];
                    }
                    if (colaMaquina.Count > 0)
                    {
                        Object[] colMaq = colaMaquina.ToArray();
                        for (int t = 0; t < colMaq.Length; t++)
                        {
                            tirosMaquina += colMaq[t];
                        }
                        Console.Write("Se busca: " + tirosMaquina + " tiro r: " + tirosRival);
                        Base mejorTiro = buscarMejorTiro(tirosMaquina, tirosRival);
                        if (mejorTiro != null)
                        {
                            Char[] tiroSeparado = mejorTiro.getGano().ToCharArray();

                            tablero[Convert.ToInt32((tiroSeparado[colaMaquina.Count].ToString()))] = 0;
                            colaMaquina.Enqueue(Convert.ToInt32(tiroSeparado[colaMaquina.Count].ToString()));
                        }
                        else
                        {
                            int t = tiroAzar();
                            if (t != (-1))
                            {
                                tablero[t] = 0;
                                colaMaquina.Enqueue(t);
                                if (deboComparar(5))
                                {
                                    if (seGano(0))
                                    {
                                        Console.Write("Se gano con maquina");
                                        pintarTablero();
                                        almacenarResultado(false, "BASECONOCIMIENTO_GATO.TXT");

                                    }
                                    else
                                    {
                                        if (hayTiros())
                                        {

                                            jugarHumano();
                                        }
                                        else
                                        {
                                            pintarTablero();
                                        }
                                    }

                                }
                                else
                                {

                                    jugarHumano();

                                }


                            }

                        }


                    }
                    else
                    {
                        Base mejorTiro = buscarMejorTiro("", tirosRival);
                        if (mejorTiro != null)
                        {
                            Char[] tiroSeparado = mejorTiro.getGano().ToCharArray();

                            tablero[Convert.ToInt32((tiroSeparado[colaMaquina.Count].ToString()))] = 0;
                            colaMaquina.Enqueue(Convert.ToInt32(tiroSeparado[colaMaquina.Count].ToString()));
                        }
                        else
                        {
                            int t = tiroAzar();
                            if (t != (-1))
                            {
                                tablero[t] = 0;
                                colaMaquina.Enqueue(t);
                                if (deboComparar(5))
                                {
                                    if (seGano(0))
                                    {
                                        Console.WriteLine("Se gano con maquina");
                                        pintarTablero();
                                        almacenarResultado(false, "BASECONOCIMIENTO_GATO.TXT");

                                    }
                                    else
                                    {
                                        if (hayTiros())
                                        {

                                            jugarHumano();
                                        }
                                        else
                                        {
                                            pintarTablero();
                                        }
                                    }

                                }
                                else
                                {

                                    jugarHumano();

                                }


                            }

                        }

                    }

                    if (deboComparar(5))
                    {
                        if (seGano(0))
                        {
                            Console.Write("Se gano con maquina");
                            pintarTablero();
                            almacenarResultado(false, "BASECONOCIMIENTO_GATO.TXT");

                        }
                        else
                        {
                            if (hayTiros())
                            {

                                jugarHumano();
                            }
                            else
                            {
                                Console.Write("Se termino");
                                pintarTablero();
                            }
                        }



                    }
                    else
                    {

                        jugarHumano();

                    }
                }
                else
                {
                    Base mejorTiro = buscarMejorTiro();
                    Char[] tiroSeparado = mejorTiro.getGano().ToCharArray();


                    tablero[Convert.ToInt32((tiroSeparado[colaMaquina.Count].ToString()))] = 0;
                    colaMaquina.Enqueue(Convert.ToInt32(tiroSeparado[colaMaquina.Count].ToString()));

                    if (deboComparar(5))
                    {
                        if (seGano(0))
                        {
                            Console.Write("Se gano con maquina");
                            pintarTablero();
                            almacenarResultado(false, "BASECONOCIMIENTO_GATO.TXT");

                        }
                        else
                        {
                            if (hayTiros())
                            {
                                jugarHumano();
                            }
                            else
                            {
                                pintarTablero();
                            }
                        }

                    }
                    else
                    {

                        jugarHumano();

                    }


                }

            }
            else
            {
                int t = tiroAzar();
                if (t != (-1))
                {
                    tablero[t] = 0;
                    colaMaquina.Enqueue(t);
                    if (deboComparar(5))
                    {
                        if (seGano(0))
                        {
                            Console.Write("Se gano con maquina");
                            pintarTablero();
                            almacenarResultado(false, "BASECONOCIMIENTO_GATO.TXT");

                        }
                        else
                        {
                            if (hayTiros())
                            {

                                jugarHumano();
                            }
                            else
                            {
                                pintarTablero();
                            }
                        }

                    }
                    else
                    {

                        jugarHumano();

                    }


                }

            }
        }

        public void jugarMaquinaMini()
        {
            pintarTablero();
            incrementarCantidadTiros();
            int tiroBueno = Tirar(mot);
            int[] posicion = conviertePos(tiroBueno);
            mot.tirar(posicion[1], posicion[0], 'O');


            tablero[tiroBueno] = 0;
            if (deboComparar(5))
            {
                if (seGano(0))
                {
                    pintarTablero();
                    Console.Write("Gano la poderosa Máquina.");
                }
                else
                {
                    if (hayTiros())
                    {
                        jugarHumanoMini();
                    }
                    else
                    {
                        pintarTablero();
                        Console.Write("Se termino en un lamentable empate.");
                       
                    }
                }
                        



          }else
            {

                jugarHumanoMini();

            }
                    

        }


        public void jugar()
        {
            humanoTiras = humanoTira();
            if (humanoTiras)
            {

                jugarHumano();

            }
            else
            {

                jugarMaquina();
            }

        }

        public void jugarMini()
        {
            humanoTiras = humanoTira();
            if (humanoTiras)
            {

                jugarHumanoMini();

            }
            else
            {

                jugarMaquinaMini();
            }

        }

        public void almacenarResultado(Boolean ganoHumano, String path)
        {

            if (colaMaquina.Count > 0)
            {

                Object a = colaMaquina.Dequeue();
                String humano = "";
                String maquina = "";
                maquina += a.ToString();

                while (a != null)
                {
                    Console.Write(a);
                    if (colaMaquina.Count > 0)
                    {
                        a = colaMaquina.Dequeue();
                        maquina += a.ToString();
                    }
                    else
                    {
                        a = null;
                    }

                }
                Console.Write("|");
                Object b = colaHumano.Dequeue();
                humano += b.ToString();
                while (b != null)
                {
                    Console.Write(b);
                    if (colaHumano.Count > 0)
                    {
                        b = colaHumano.Dequeue();
                        humano += b.ToString();
                    }
                    else
                    {
                        b = null;
                    }
                }
                String escribir = "";
                if (ganoHumano)
                {

                    Boolean encontrado = false;
                    for (int t = 0; t < baseConocimientos.Length; t++)
                    {
                        if (baseConocimientos[t].getGano().Equals(humano) && baseConocimientos[t].getPerdio().Equals(maquina))
                        {
                            baseConocimientos[t].setCantidad(baseConocimientos[t].getCantidad() + 1);
                            encontrado = true;
                        }
                        if (t + 1 == baseConocimientos.Length)
                        {
                            escribir += baseConocimientos[t].getGano() + "|" + baseConocimientos[t].getPerdio() + "|" + baseConocimientos[t].getResultado() + "|" + baseConocimientos[t].getCantidad();
                        }
                        else
                        {
                            escribir += baseConocimientos[t].getGano() + "|" + baseConocimientos[t].getPerdio() + "|" + baseConocimientos[t].getResultado() + "|" + baseConocimientos[t].getCantidad() + "\n";
                        }

                    }
                    if (!encontrado)
                    {
                        escribir += "\n";
                        escribir += humano + "|" + maquina + "|G|" + "1";
                    }

                }
                else
                {
                    Boolean encontrado = false;
                    for (int t = 0; t < baseConocimientos.Length; t++)
                    {
                        if (baseConocimientos[t].getGano().Equals(maquina) && baseConocimientos[t].getPerdio().Equals(humano))
                        {
                            baseConocimientos[t].setCantidad(baseConocimientos[t].getCantidad() + 1);
                            encontrado = true;
                        }
                        if (t + 1 == baseConocimientos.Length)
                        {
                            escribir += baseConocimientos[t].getGano() + "|" + baseConocimientos[t].getPerdio() + "|" + baseConocimientos[t].getResultado() + "|" + baseConocimientos[t].getCantidad();
                        }
                        else
                        {
                            escribir += baseConocimientos[t].getGano() + "|" + baseConocimientos[t].getPerdio() + "|" + baseConocimientos[t].getResultado() + "|" + baseConocimientos[t].getCantidad() + "\n";
                        }
                    }
                    if (!encontrado)
                    {
                        escribir += "\n";
                        escribir += maquina + "|" + humano + "|G|" + "1";
                    }
                }

                StreamWriter outfile = new StreamWriter(path);
                Console.Write("Escribiendo");
                outfile.Write(escribir);
                outfile.Close();
            }
        }
        //metodo que convierte posiciones de numeros a una coordenada
        public int[] conviertePos(int pos) {
            int[] arrPos= new int[2];
            switch (pos) { 
                case 0 :
                    arrPos[0]=0;
                    arrPos[1]=0;
                    return arrPos;
                    
                case 1:
                    arrPos[0]=0;
                    arrPos[1]=1;
                    return arrPos;
                case 2:
                    arrPos[0] = 0;
                    arrPos[1] = 2;
                    return arrPos;
                case 3:
                    arrPos[0] = 1;
                    arrPos[1] = 0;
                    return arrPos;
                  
                case 4:
                    arrPos[0] = 1;
                    arrPos[1] = 1;
                    return arrPos;
                  
                case 5:
                    arrPos[0] = 1;
                    arrPos[1] = 2;
                    return arrPos;
                  
                case 6:
                    arrPos[0] = 2;
                    arrPos[1] = 0;
                    return arrPos;
                  
                case 7:
                    arrPos[0] = 2;
                    arrPos[1] = 1;
                    return arrPos;
                  
                case 8:
                    arrPos[0] = 2;
                    arrPos[1] = 2;
                    return arrPos;
                  
                default: 
                    arrPos[0] = -1;
                    arrPos[1] = -1;
                    return arrPos;
            
            }
        } 

        static void Main()
        {
            Gato g = new Gato(2);
            Console.WriteLine("Que comience el juego---");
            g.jugarMini();

            Console.ReadKey();
         
            
            Console.ReadKey();


        }
    }
    class Base
    {
        private String gano;
        private String perdio;
        private String resultado;
        private int cantidad;

        public void setGano(String g)
        {
            gano = g;
        }
        public void setPerdio(String p)
        {
            perdio = p;
        }
        public void setResultado(String r)
        {
            resultado = r;
        }
        public void setCantidad(int cantidad)
        {
            this.cantidad = cantidad;
        }
        public String getGano()
        {
            return gano;
        }
        public String getPerdio()
        {
            return perdio;
        }
        public String getResultado()
        {
            return resultado;
        }
        public int getCantidad()
        {
            return cantidad;
        }


    }
}