using System;
using System.Data.SqlClient;
using System.IO;

namespace ConsultasBD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Caminho do arquivo CSV
            string pastaDestino = @"Caminho da pasta";
            string caminhoArquivo = Path.Combine(pastaDestino, "relatorio.csv");

            if (!Directory.Exists(pastaDestino))
            {
                Directory.CreateDirectory(pastaDestino);
            }
            // Realiza a consulta ao banco de dados e gera o arquivo CSV
            ConsultarBancoDeDados(caminhoArquivo);

            if (File.Exists(caminhoArquivo))
            {
                // Envia o relatório por e-mail
                EnviarEmail.EnviarRelatorio(caminhoArquivo);
            }
            else
            {
                Console.WriteLine("O arquivo de relatório não foi encontrado.");
            }
        }

        static void ConsultarBancoDeDados(string caminhoArquivo)
        {
            
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=AdventureWorksLT2022;Trusted_Connection=True;";

            // Parâmetros da consulta
            string categoriaFiltro = "Categoria B";

            //Consulta SQL a ser executada com parâmetros
            string query = @"
                SELECT id, nome, quantidade, valor, categoria 
                FROM Produtos 
                WHERE categoria = @Categoria ORDER BY ID ASC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexão aberta com sucesso!");

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Categoria", categoriaFiltro);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            using (StreamWriter writer = new StreamWriter(caminhoArquivo))
                            {
                                // Cabeçalhos do CSV
                                writer.WriteLine("ID,Nome,Quantidade,Valor,Categoria");

                                // Lendo e escrevendo os resultados no arquivo CSV
                                while (reader.Read())
                                {
                                    writer.WriteLine($"{reader["id"]},{reader["nome"]},{reader["quantidade"]},{reader["valor"]},{reader["categoria"]}");
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Erro ao acessar o banco de dados: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Conexão fechada.");
                }
            }
        }
    }
}
