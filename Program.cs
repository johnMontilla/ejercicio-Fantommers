using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace prueba_tecnica
{
    class Program
    {
       
        static void Main(string[] args)
        {
            List<Persona> listaPersonas = new List<Persona>();
            string line;
            string[] datos;
             
            //leer el archivo
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"d:\TestDaiana.txt");
            while ((line = file.ReadLine()) != null)
            {
                datos = line.Split(',');                
                string NewString = Regex.Replace(datos[3], @"\s", "");
                listaPersonas.Add(new Persona { Nombre = datos[0], Dni = datos[1], Altura = Convert.ToDecimal( datos[2]), Estado= (NewString == "ACTIVO") ? true:false}); ;                             
            }
            file.Close();


            Coneccion coneccion = new Coneccion();

            //insertar en BBDD
            foreach(Persona persona in listaPersonas)
            {
                coneccion.Insertar(persona.Nombre, persona.Dni, persona.Altura, persona.Estado);
            }

            //consultar BBDD
            List<Persona> listaConsulta = coneccion.Consultar();
            
            //filtrado con linq
            IEnumerable<Persona> res = from Persona in listaConsulta where Persona.Altura > 7.56m && Persona.Estado == true select Persona;

            //convertir a JSON
            foreach (Persona persona1 in res)
            {                
                string jsonData = JsonConvert.SerializeObject(persona1);                
                Console.Write(jsonData);
            }
        }        
    }

    class Coneccion
    {
        public static MySqlConnection bd = new MySqlConnection("database=lista; Data Source=localhost; user id=root;password=1234;port=3306");
        public void Insertar(string nombre , string dni, decimal altura, bool estado)
        {
            using (bd)
            {
                try
                {
                    bd.Open();
                    MySqlCommand query = new MySqlCommand("INSERT INTO persona (nombre, dni, altura, estado)VALUES('" + nombre + "','" + dni + "','" + altura + "','"+estado+"');", bd);
                    query.ExecuteNonQuery();
                    bd.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public List<Persona> Consultar()
        {
            MySqlDataReader datos;
            List<Persona> lista = new List<Persona>();
            using (bd)
            {
                try
                {
                    bd.Open();
                    MySqlCommand query = new MySqlCommand("SELECT * FROM persona;", bd);
                    datos = query.ExecuteReader();
                    if (datos.HasRows)
                    {
                        while (datos.Read())
                        {
                            lista.Add(new Persona { Nombre = datos.GetString(0), Dni = datos.GetString(1), Altura = datos.GetDecimal(2), Estado = (datos.GetString(3) == "True") ? true : false });
                        }
                    }
                    return lista;
                }
                finally
                {
                    bd.Close();
                }

            }
        }
    }
   

    class Persona
    {
        public string Nombre { get; set; }
        public string Dni { get; set; }
        public decimal Altura { get; set; }
        public bool Estado { get; set; }       
    }
}
