using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GatoConsola
{
    public class Motor
    {

        /*
         * Esta clase es la que controla el funcionamiento
         * */
        bool tX = true;
        char[,] casillas; // La matriz de casillas
        Point[,] formasDeGanar;
        public Motor()
        {
            casillas = new char[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    casillas[i, j] = ' ';
                }
            }
            formasDeGanar = new Point[8, 3];
            formasDeGanar[0, 0] = new Point(0, 0);
            formasDeGanar[0, 1] = new Point(0, 1);
            formasDeGanar[0, 2] = new Point(0, 2);
            formasDeGanar[1, 0] = new Point(1, 0);
            formasDeGanar[1, 1] = new Point(1, 1);
            formasDeGanar[1, 2] = new Point(1, 2);
            formasDeGanar[2, 0] = new Point(2, 0);
            formasDeGanar[2, 1] = new Point(2, 1);
            formasDeGanar[2, 2] = new Point(2, 2);
            formasDeGanar[3, 0] = new Point(0, 0);
            formasDeGanar[3, 1] = new Point(1, 0);
            formasDeGanar[3, 2] = new Point(2, 0);
            formasDeGanar[4, 0] = new Point(0, 1);
            formasDeGanar[4, 1] = new Point(1, 1);
            formasDeGanar[4, 2] = new Point(2, 1);
            formasDeGanar[5, 0] = new Point(0, 2);
            formasDeGanar[5, 1] = new Point(1, 2);
            formasDeGanar[5, 2] = new Point(2, 2);
            formasDeGanar[6, 0] = new Point(0, 0);
            formasDeGanar[6, 1] = new Point(1, 1);
            formasDeGanar[6, 2] = new Point(2, 2);
            formasDeGanar[7, 0] = new Point(0, 2);
            formasDeGanar[7, 1] = new Point(1, 1);
            formasDeGanar[7, 2] = new Point(2, 0);

        }

        public bool estaOcupuada(int x, int y)
        {
            return casillas[y, x] != ' ';
        }

        public bool estaVacia(int x, int y)
        {
            return casillas[y, x] == ' ';
        }

        //cambia turnos y verifica turnos
        public void tirar(int x, int y, char jugador)
        {
            if ((jugador == 'X' && tX) || (jugador == 'O' && !tX))
            {
                casillas[y, x] = jugador;
                tX = !tX;
            }
        }

        public void borrar(int x, int y)
        {
            if ((casillas[y, x] == 'X' && !tX) || (casillas[y, x] == 'O' && tX))
            {
                casillas[y, x] = ' ';
                tX = !tX;
            }
        }

        public char getCasilla(int x, int y)
        {
            return casillas[y, x];
        }

        public bool esTurnoX()
        {
            return tX;
        }

        public bool esTurnoO()
        {
            return !tX;
        }

        //Este metodo determina el estado del juego, si hay ganador, es empate o aun se esta jugando.
        private int quienGana()
        {
            for (int i = 0; i < 8; i++)
            {

                if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 1].Y, formasDeGanar[i, 1].X] &&
                    casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == casillas[formasDeGanar[i, 2].Y, formasDeGanar[i, 2].X])
                {
                    if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'X')
                    {
                        return 1;
                    }
                    else if (casillas[formasDeGanar[i, 0].Y, formasDeGanar[i, 0].X] == 'O')
                    {
                        return 2;
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (casillas[i, j] == ' ') return 0;
                }
            }

            return 3;
        }



        public int getCasillasVacias()//Cuenta cuantas casillas estan vacias
        {
            int x = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (estaVacia(j, i)) x++;
                }
            }
            return x;
        }

        public bool hayGanador() //Estos metodos funcionan dependiendo del resultado del metodo anterior.
        {
            int x = quienGana();
            return (x == 1 || x == 2);
        }

        public bool gananX()
        {
            return quienGana() == 1;
        }

        public bool gananO()
        {
            return quienGana() == 2;
        }

        public bool empate()
        {
            return quienGana() == 3;
        }

        public bool juegoEnCurso()
        {
            return quienGana() == 0;
        }

        public int getPuntos2()
        {
            if (hayGanador())
            {
                if (gananO()) return -1;
                else return 1;
            }
            else
                return 0;
        }

        public int getPuntos()
        {
            int x = 0;
            int o = 0;
            int total = 0;
            for (int i = 0; i < 8; i++)
            {
                o = x = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (casillas[formasDeGanar[i, j].Y, formasDeGanar[i, j].X] == 'X')
                        x++;
                    else if (casillas[formasDeGanar[i, j].Y, formasDeGanar[i, j].X] == 'O')
                        o++;
                }
                if (x == 0)
                {
                    switch (o)
                    {
                        case 1: total -= 1; break;
                        case 2: total -= 4; break;
                        case 3: total -= 32; break;
                    }
                }
                else if (o == 0)
                {
                    switch (x)
                    {
                        case 1: total += 1; break;
                        case 2: total += 4; break;
                        case 3: total += 32; break;
                    }
                }
            }

            return total;
        }


    }
}
