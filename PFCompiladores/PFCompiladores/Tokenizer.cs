using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFCompiladores
{
    class Tokenizer
    {
        private string aTexto;
        private ArrayList aLista;
        private int aIndice;
       
        /* ========================================================== */
        public Tokenizer(string pTexto)
        {
            /* ----- Recuperar texto */
            aTexto = pTexto;
            /* ----- Generar Lista de Tokens */
            GenerarListaTokens(" \n\r\t ", true);
        }
        /* ========================================================== */
        public Tokenizer(string pTexto, string pDelimitadores)
        {
            /* ----- Recuperar texto */
            aTexto = pTexto;
            /* ----- Generar Lista de Tokens */
            GenerarListaTokens(pDelimitadores, true);
        }
        /* ========================================================== */
        public Tokenizer(string pTexto, string pDelimitadores, bool RetornaTokens)
        {
            /* ----- Recuperar texto */
            aTexto = pTexto;
            /* ----- Generar Lista de Tokens */
            GenerarListaTokens(pDelimitadores, RetornaTokens);
        }
        /* ====================================================================== */
        private void GenerarListaTokens(string Delimitadores, bool RetornaTokens)
        {
            aLista = new ArrayList();
            /* Convertir Texto en un arreglo de caracteres */
            char[] Caracteres = aTexto.ToCharArray();
            /* Revisar caracter por caracter para formar Tokens */
            string Token = "";
            for (int K = 0; K < Caracteres.Length; K++)
            {
                if (Delimitadores.IndexOf(Caracteres[K]) == -1)
                    Token = Token + Caracteres[K].ToString();
                else
                {
                    /* ----- Agregar Token al array */
                    if (!Token.Equals(""))
                        aLista.Add(Token);
                    /* ----- Agregar Delimitador al array */
                    if (RetornaTokens)
                        aLista.Add(Caracteres[K].ToString());
                    /* ----- Inicializar Token */
                    Token = "";
                }
            }
            /* ----- Agregar ultimo Token al array, si existe */
            if (!Token.Equals(""))
                aLista.Add(Token);
            /* ----- Inicializar Indice */
            aIndice = 0;
        }
        /* ====================================================================== */
        public int CountTokens()
        {
            return aLista.Count;
        }
        /* ====================================================================== */
        public bool HasMoreTokens()
        {
            return (aIndice < aLista.Count);
        }
        /* ====================================================================== */
        public string NextToken()
        {
            if (aIndice < aLista.Count)
                return aLista[aIndice++].ToString();
            else
                return null;
        }
    }
}
