using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFCompiladores
{
    class Program
    {
        static int contador_resultado = 0;
        static int contador_lineas = 0;
        static int contador_tab = 0;

        // Recuperar la ruta donde estan los archivos .txt
        static string RutaDatos()
        {
            string Ruta = Directory.GetCurrentDirectory();
            int Indice = Ruta.IndexOf("PFCompiladores\\bin");
            Ruta = Ruta.Substring(0, Indice - 1) + @"\Datos";
            return Ruta;
        }

        // Recuperar datos del archivo .txt
        static string LeerDatos(string archivo)
        {
            string C = File.ReadAllText(archivo);
            return C;
        }
      
        // Tokenizar el codigo del archivo .txt
        static List<List<string>> Trozar(string S)
        {
            
            List<List<string>> L = new List<List<string>>();
            string Registro;
            StreamReader Archivo = new StreamReader(S);
            Registro = Archivo.ReadLine();
            while (Registro != null)
            {
                List<string> temp = new List<string>();
                Tokenizer T = new Tokenizer(Registro, " ,:=><+-*%/()\t");
                while (T.HasMoreTokens())
                {                   
                    temp.Add(T.NextToken());                                                                               
                }
                
                for (int k = 0; k < temp.Count; k++)
                {

                    if (temp[k] == "\t")
                        temp[k] = "tab";
                }                             
                
                temp = temp.Where(x => x != " " && x != "\n").ToList();
                if (temp.Count() != 0)
                {
                    L.Add(temp);
                }
                Registro = Archivo.ReadLine();
            }
            Archivo.Close();
            

            return L;  
        }
        
        static void Listar(List<List<string>> L)
        {
            int num_linea = 0;
            foreach(List<string> l in L)
            {
                Console.Write($"{num_linea}:");
                foreach(string s in l)
                {
                    Console.Write($"{s} ");
                }
                Console.WriteLine();
                num_linea += 1;
            }
        }

        //modulo para calcular el final
        static int Final_Operacion(List<List<string>> Codigo, int posInicial)
        {
            int posFinal = posInicial;
            //recorrer desde psoInicial hasta todo
            int temp = contador_tab - 1;
            for (int i = posInicial; i < Codigo.Count; i++)
            {
                if (Codigo[i][temp] == "tab")
                {
                    posFinal = i; 
                }
                else
                {
                    break;
                }
            }
            
            return posFinal;
        }
        //---------------------------------Posfijo-----------------------------------------------
        //esta funcion recibe una lista de strings y devuelve una lista de listas de strings
        static List<List<string>> analizador_una_linea(List<string> stringeninfijo)
        {
            //numero de elementos de la lista
            int tamanio = stringeninfijo.Count;
            //iniciar lista de simbolos
            string Signos = "=><+-*/%()";

            Stack<string> pila = new Stack<string>();
            Stack<string> pilaR = new Stack<string>();
            StringBuilder stringenposfijo = new StringBuilder();
            List<List<string>> R = new List<List<string>>();

            //recorrer toda la expresion en infijo
            for (int i = 0; i < tamanio; i++)
            {
                //comparar si son id's
                if (!Signos.Contains(stringeninfijo[i]))
                {
                    stringenposfijo.Append(stringeninfijo[i]);
                    pilaR.Push(stringeninfijo[i]);
                }
                else if (stringeninfijo[i] == "(")
                {
                    pila.Push(stringeninfijo[i]);
                }
                else if ((stringeninfijo[i] == "*") || (stringeninfijo[i] == "+") || (stringeninfijo[i] == "-") || (stringeninfijo[i] == "/") ||
                        (stringeninfijo[i] == "<") || (stringeninfijo[i] == ">") || (stringeninfijo[i] == "%") || (stringeninfijo[i] == "="))
                {
                    while ((pila.Count > 0) && (pila.Peek() != "("))
                    {
                        if (precedenciadeoperadores(pila.Peek(), stringeninfijo[i]))
                        {
                            string aux = pila.Pop();
                            stringenposfijo.Append(aux);
                            pilaR.Push(aux);
                            if (pilaR.Peek() == "=")
                            {
                                //imprimir resultado
                                string operacion = pilaR.Pop();
                                string arg1 = pilaR.Pop();
                                string arg2 = pilaR.Pop();

                                //agregar a R
                                List<string> L_aux = new List<string>() { "asignar", arg1, "vacio", arg2 };
                                R.Add(L_aux);                           

                            }
                            else
                            {
                                //imprimir resultado
                                string operacion = pilaR.Pop();
                                string arg2 = pilaR.Pop();
                                string arg1 = pilaR.Pop();
                                contador_resultado += 1;
                                string numResultado = "R" + contador_resultado.ToString();
                                pilaR.Push(numResultado);

                                //agregar a R
                                List<string> L_aux = new List<string>() { operacion, arg1, arg2, numResultado };
                                R.Add(L_aux);
                        
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                    pila.Push(stringeninfijo[i]);
                }
                else if (stringeninfijo[i] == ")")
                {
                    while ((pila.Count > 0) && (pila.Peek() != "("))
                    {
                        string aux = pila.Pop();
                        stringenposfijo.Append(aux);
                        pilaR.Push(aux);

                        if (pilaR.Peek() == "=")
                        {
                            //imprimir resultado
                            string operacion = pilaR.Pop();
                            string arg1 = pilaR.Pop();
                            string arg2 = pilaR.Pop();

                            //agregar a R
                            List<string> L_aux = new List<string>() { "asignar", arg1, "vacio", arg2 };
                            R.Add(L_aux);                            

                        }
                        else
                        {
                            //imprimir resultado
                            string operacion = pilaR.Pop();
                            string arg2 = pilaR.Pop();
                            string arg1 = pilaR.Pop();
                            contador_resultado += 1;
                            string numResultado = "R" + contador_resultado.ToString();
                            pilaR.Push(numResultado);

                            //agregar a R
                            List<string> L_aux = new List<string>() { operacion, arg1, arg2, numResultado };
                            R.Add(L_aux);
                        }
                    }
                    if (pila.Count > 0)
                        pila.Pop(); //quita el parentesis izquierdo de la pila
                }
            }
            while (pila.Count > 0)
            {
                string aux = pila.Pop();
                stringenposfijo.Append(aux);
                pilaR.Push(aux);

                if (pilaR.Peek() == "=")
                {
                    //imprimir resultado
                    string operacion = pilaR.Pop();
                    string arg1 = pilaR.Pop();
                    string arg2 = pilaR.Pop();

                    //agregar a R
                    List<string> L_aux = new List<string>() { "asignar", arg1, "vacio", arg2 };
                    R.Add(L_aux);               

                }
                else
                {
                    //imprimir resultado
                    string operacion = pilaR.Pop();
                    string arg2 = pilaR.Pop();
                    string arg1 = pilaR.Pop();
                    contador_resultado += 1;
                    string numResultado = "R" + contador_resultado.ToString();
                    pilaR.Push(numResultado);

                    //agregar a R
                    List<string> L_aux = new List<string>() { operacion, arg1, arg2, numResultado };
                    R.Add(L_aux);
                   
                }
            }
            return R;
        }
        static bool precedenciadeoperadores(string top, string p_2)
        {
            if (top == ">" && p_2 == "*")
                return false;
            if (top == ">" && p_2 == "/")
                return false;
            if (top == ">" && p_2 == "%")
                return false;
            if (top == ">" && p_2 == "+")
                return false;
            if (top == ">" && p_2 == "-")
                return false;
            if (top == "<" && p_2 == "*")
                return false;
            if (top == "<" && p_2 == "/")
                return false;
            if (top == "<" && p_2 == "%")
                return false;
            if (top == "<" && p_2 == "+")
                return false;
            if (top == "<" && p_2 == "-")
                return false;

            if (top == "+" && p_2 == "*")
                return false;
            if (top == "+" && p_2 == "/")
                return false;
            if (top == "+" && p_2 == "%")
                return false;
            if (top == "-" && p_2 == "*")
                return false;
            if (top == "-" && p_2 == "/")
                return false;
            if (top == "-" && p_2 == "%")
                return false;

            if (top == "=" && p_2 == "*")
                return false;
            if (top == "=" && p_2 == "/")
                return false;
            if (top == "=" && p_2 == "+")
                return false;
            if (top == "=" && p_2 == "-")
                return false;
            if (top == "=" && p_2 == "%")
                return false;

            return true;
        }


        //conversor de codigo intermedio, retorna la lista con las lineas de cod intermedio
        static List<List<string>> Generador(List<List<string>> Codigo, int posInicial, int posFinal)
        {
            List<List<string>> R = new List<List<string>>();
            //i son las filas
            int i = posInicial;
            
            while (i <= posFinal)
            {
                
                //Console.WriteLine("i =" + i);
                string PrimerElemento = Codigo[i][contador_tab];
               

                switch (PrimerElemento)
                {
                    case "if":
                        //Aumentar el numero de tab en 1
                        contador_tab += 1;

                        //--------Parte 1--------
                        //realizar acciones de la linea if y añadir a R
                        //    if,(,a,<,b,),:
                        //obtener la expresion simple
                        List<string> linea_simple_if = new List<string>();
                        for (int j = contador_tab; j < Codigo[i].Count(); j++)
                        {
                            if (Codigo[i][j] == ":")
                            {
                                break;
                            }
                            //agregar los valores de la linea simple: (,a,<,b,)
                            linea_simple_if.Add(Codigo[i][j]);

                        }

                        //analiza la expresion simple (,a,<,b,)
                        List<List<string>> linea_if = analizador_una_linea(linea_simple_if);
                        //agregar linea_if a R
                        foreach (List<string> linea in linea_if)
                        {
                            R.Add(linea);
                            contador_lineas += 1;
                        }

                        //--------Parte 2--------
                        //calcular la posicion final
                        int final_if = Final_Operacion(Codigo, i + 1);
                        

                        //Almacenar el numero de resultado actaul
                        int temp = contador_resultado;

                        //llamar a generador para la seccion if
                        List<List<string>> dentro_if = Generador(Codigo, i + 1, final_if);
                        
                        //actualizar el valor de la linea a recorrer
                        i = final_if + 1;

                        //aumentar contador de lineas por la linea de escero
                        contador_lineas += 1;
                        //verificar si el siguiente elemento es un else
                        int desfase_lineas = 0;
                        if ((i < posFinal) && (Codigo[i][contador_tab - 1] == "else"))
                        {
                            desfase_lineas = 1;
                        }
                        int temp_salto_if = contador_lineas + contador_tab - 1 + desfase_lineas;

                        List<string> Sentencia_If = new List<string>() { "esCero", "R" + temp.ToString(),
                            "vacio", temp_salto_if.ToString() };

                        //agregar a R
                        R.Add(Sentencia_If);

                        //Añadir lo que esta dentro_if a R
                        foreach (List<string> linea in dentro_if)
                        {
                            R.Add(linea);
                        }

                        contador_tab -= 1;
                        break;

                    case "else":
                        contador_tab += 1;
                        //realizar acciones de la linea else y añadir a R
                        //    else:
                        //calcular la posicion final
                        int final_else = Final_Operacion(Codigo, i + 1);
                        

                        //llamar a generador para la seccion if
                        List<List<string>> dentro_else = Generador(Codigo, i + 1, final_else);
                        //actualizar el valor de i
                        i = final_else + 1;

                        //añadir antes de añadir los otros
                        contador_lineas += 1;
                        int temp_salto_else = contador_lineas + contador_tab - 1;

                        List<string> Sentencia_else = new List<string>() { "saltar", " ", " ", temp_salto_else.ToString() };

                        //Sentencia_else agregar a R
                        R.Add(Sentencia_else);

                        //Añadir lo que esta dentro del if a R
                        foreach (List<string> linea in dentro_else)
                        {
                            R.Add(linea);
                        }
                        contador_tab -= 1;
                        break;

                    case "while":
                        //amentar el tab en 1
                        contador_tab += 1;
                        //realizar acciones de la linea while y añadir a R
                        List<string> linea_simple_while = new List<string>();
                        for (int j = contador_tab; j < Codigo[i].Count(); j++)
                        {
                            if (Codigo[i][j] == ":")
                            {
                                break;
                            }
                            //agregar los valores de la linea simple: (,a,<,100,)
                            linea_simple_while.Add(Codigo[i][j]);

                        }

                        //analiza la expresion simple (,a,<,100,)
                        List<List<string>> linea_while = analizador_una_linea(linea_simple_while);


                        //agregar linea_while a R
                        foreach (List<string> linea in linea_while)
                        {
                            R.Add(linea);
                            contador_lineas += 1;
                        }
                        int temp2_while = contador_lineas - 1;

                        //calcular la posicion final
                        int final_while = Final_Operacion(Codigo, i + 1);
                        
                        int temp1_while = contador_resultado;

                        //llamar a generador para la seccion while
                        List<List<string>> dentro_while = Generador(Codigo, i + 1, final_while);

                        //actualizar el valor de i
                        i = final_while + 1;

                        //incrementamos un 1 en total de lineas por la linea saltar del final del bucle
                        contador_lineas += 1;

                        //calculamos la linea saltar final
                        temp2_while = temp2_while + contador_tab - 1; 
                        List<string> Sentencia_saltar_while = new List<string>() { "Saltar", " ",
                            " ", temp2_while.ToString() };

                        //incrementamos un 1 e ntotal de lineas por la linea escero
                        contador_lineas += 1;
                        int temp_salto_while = contador_lineas + contador_tab - 1 ;

                        //calculaamos la linea escero
                        List<string> Sentencia_while = new List<string>() { "esCero", "R" + temp1_while.ToString(),
                            "vacio", temp_salto_while.ToString() };

                        //agregar a R
                        R.Add(Sentencia_while);

                        //Añadir lo que esta dentro del while a R
                        foreach (List<string> linea in dentro_while)
                        {
                            R.Add(linea);
                        }

                        //Añadir la sentencia saltar de nuevo al bucle
                        R.Add(Sentencia_saltar_while);

                        //disminuir tab
                        contador_tab -= 1;
                        break;

                    case "for":
                        //aumentar el tab en 1
                        contador_tab += 1;

                        //obtener valores de limite y sumatoria
                        string variable = Codigo[i][contador_tab];
                        string limite_inferior = Codigo[i][contador_tab + 4];
                        string limite_superior = Codigo[i][contador_tab + 6];
                        string incremento = Codigo[i][contador_tab + 8];

                        R.Add(new List<string>() { "asignar", limite_inferior, "vacio", variable });

                        contador_resultado += 1;
                        R.Add(new List<string>() { "<", variable, limite_superior, "R" + contador_resultado.ToString() });

                        contador_lineas += 2;


                        int temp2_for = contador_lineas - 1;

                        //calcular la posicion final
                        int final_for = Final_Operacion(Codigo, i + 1);
                        

                        int temp1_for = contador_resultado;

                        //llamar a generador para la seccion for
                        List<List<string>> dentro_for = Generador(Codigo, i + 1, final_for);

                        //actualizar el valor de i
                        i = final_for + 1;


                        //incrementamos un 1 en total de lineas por la linea saltar del final del bucle
                        contador_lineas += 1;

                        //calculamos la linea saltar final
                        temp2_for = temp2_for + contador_tab - 1;
                        List<string> Sentencia_saltar_for = new List<string>() { "Saltar", " ",
                            " ", temp2_for.ToString() };

                        //incrementamos un 1 e ntotal de lineas por la linea escero
                        contador_lineas += 1;
                        int temp_salto_for = contador_lineas + contador_tab - 1 + 2;

                        //calculaamos la linea escero
                        List<string> Sentencia_for = new List<string>() { "esCero", "R" + temp1_for.ToString(),
                            "vacio", temp_salto_for.ToString() };

                        //agregar a R
                        R.Add(Sentencia_for);

                        //Añadir lo que esta dentro del while a R
                        foreach (List<string> linea in dentro_for)
                        {
                            R.Add(linea);
                        }

                        //añadir sentencias para el contador
                        contador_resultado += 1;
                        R.Add(new List<string>() { "+", variable, incremento, "R" +  contador_resultado.ToString()});
                        R.Add(new List<string>() { "asignar", "R" + contador_resultado.ToString(), "vacio",variable });

                        contador_lineas += 2;

                        //Añadir la sentencia saltar de nuevo al bucle
                        R.Add(Sentencia_saltar_for);

                        //disminuir tab
                        contador_tab -= 1;
                        break;

                    default:
                        //analizar la sentencias linea por linea
                        List<string> linea_simple_nada = new List<string>();
                        for (int j = contador_tab ; j < Codigo[i].Count(); j++)
                        {
                            //agregar los valores de la linea simple: (,a,<,b,)
                            linea_simple_nada.Add(Codigo[i][j]);

                        }
                        List<List<string>> linea_nada = analizador_una_linea(linea_simple_nada);

                        //agregar analizador_una_linea a R
                        foreach (List<string> linea in linea_nada)
                        {
                            contador_lineas += 1;
                            R.Add(linea);
                        }

                        i += 1;
                        break;

                }
            }
            
            return R;

        }

        // Modulo para listar codigo intermedio
        static void ListarIntermedio(List<List<string>> L)
        {
            int num_linea = 0;
            foreach (List<string> l in L)
            {
                Console.Write($"{num_linea}: (");
                foreach (string s in l)
                {
                    if (s != l.Last())
                        Console.Write($"\t{s}\t,");
                    else
                    {
                        Console.Write($"\t{s}\t");
                    }
                }
                Console.Write(")");
                Console.WriteLine();
                if (l == L.Last())
                {
                    Console.WriteLine($"{num_linea + 1}: (\t\t,\t\t,\t\t,\t\t)");
                }
                num_linea += 1;
            }
        }


        static void Menu()
        {
            Console.WriteLine();
            Console.WriteLine(" --- MENU ARCHIVOS --- ");
            Console.WriteLine("1. Programa 1 (if - else)");
            Console.WriteLine("2. Programa 2 (while)");
            Console.WriteLine("3. Programa 3 (for)");
            Console.WriteLine("4. Programa 4 (mezcla)");
            Console.WriteLine("5. Salir");
        }

        static void Main(string[] args)
        {
            
            int Option = 1;
            string D;
            Console.WriteLine("*** PROGRAMA PARA CONVERTIR SEGMENTO DE PROGRAMA DE ***");
            Console.WriteLine(" ******** LENGUAJE PYTHON A CODIGO INTERMEDIO ********");
            while (Option != 0)
            {
                string Ruta;
                
                List<List<string>> E;
                List<List<string>> R = new List<List<string>>();

                Menu();
                Console.Write("Ingrese opcion -> ");
                Option = int.Parse(Console.ReadLine());
                switch (Option)
                {
                    case 1: Ruta = RutaDatos() + "\\Programa1.txt";
                        D = LeerDatos(Ruta);
                        Console.WriteLine();
                        Console.WriteLine("PROGRAMA: ");
                        Console.WriteLine(D);
                        E = Trozar(Ruta);
                        
                        Console.WriteLine("CODIGO INTERMEDIO: ");

                        R.Clear();
                        R = Generador(E, 0, E.Count() - 1);
                        ListarIntermedio(R);

                        contador_lineas = 0;
                        contador_resultado = 0;
                        break;
                    case 2: Ruta = RutaDatos() + "\\Programa2.txt";
                        D = LeerDatos(Ruta);
                        Console.WriteLine();
                        Console.WriteLine("PROGRAMA: ");
                        
                        Console.WriteLine(D);
                        E = Trozar(Ruta);
                        
                        Console.WriteLine("CODIGO INTERMEDIO: ");
                        R.Clear();
                        R = Generador(E, 0, E.Count() - 1);
                        ListarIntermedio(R);

                        contador_lineas = 0;
                        contador_resultado = 0;

                        break;
                    case 3: Ruta = RutaDatos() + "\\Programa3.txt";
                        D = LeerDatos(Ruta);
                        Console.WriteLine();
                        Console.WriteLine("PROGRAMA: ");
                        Console.WriteLine(D);
                        E = Trozar(Ruta);                       
                        Console.WriteLine("CODIGO INTERMEDIO: ");

                        R.Clear();
                        R = Generador(E, 0, E.Count() - 1);
                        ListarIntermedio(R);

                        contador_lineas = 0;
                        contador_resultado = 0;

                        break;
                    case 4:
                        Ruta = RutaDatos() + "\\Programa4.txt";
                        D = LeerDatos(Ruta);
                        Console.WriteLine();
                        Console.WriteLine("PROGRAMA: ");
                        Console.WriteLine(D);
                        E = Trozar(Ruta);
                        Console.WriteLine("CODIGO INTERMEDIO: ");
                        R.Clear();
                        R = Generador(E, 0, E.Count() - 1);
                        ListarIntermedio(R);

                        contador_lineas = 0;
                        contador_resultado = 0;

                        break;
                    case 5: Option = 0;
                        break;
                    default: Console.WriteLine("Opcion no valida");
                        contador_lineas = 0;
                        contador_resultado = 0;
                        break;
                }
            }
        }
    }
}
