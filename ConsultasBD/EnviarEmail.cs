using System;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace ConsultasBD
{
    class EnviarEmail
    {
        public static void EnviarRelatorio(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                Console.WriteLine("O arquivo de relatório não foi encontrado.");
                return;
            }

            // Lendo o conteúdo do arquivo CSV
            string[] linhas = File.ReadAllLines(caminhoArquivo);

            // Garantir que há pelo menos uma linha no arquivo
            if (linhas.Length < 2)
            {
                Console.WriteLine("O arquivo CSV está vazio ou não contém dados suficientes.");
                return;
            }

            // Criando o corpo do e-mail em HTML com novo estilo
            string emailBody = @"
            <html>
            <head>
                <style>
                    body { font-family: 'Arial', sans-serif; background-color: #f2f2f2; margin: 0; padding: 0; }
                    .container { width: 90%; max-width: 800px; margin: 20px auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); }
                    h1 { color: #333333; font-size: 28px; margin-bottom: 20px; text-align: center; }
                    table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                    th, td { padding: 12px; text-align: center; border: 1px solid #dddddd; }
                    th { background-color: #4CAF50; color: #ffffff; }
                    tr:nth-child(even) { background-color: #f9f9f9; }
                    tr:hover { background-color: #f1f1f1; }
                    .footer { text-align: center; font-size: 12px; color: #888888; margin-top: 20px; }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Relatório de Produtos</h1>
                    <table>
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Nome</th>
                                <th>Quantidade</th>
                                <th>Valor</th>
                                <th>Categoria</th>
                            </tr>
                        </thead>
                        <tbody>";

            // Adiciona os dados ao corpo do e-mail, ignorando a primeira linha (cabeçalho)
            bool isFirstLine = true;
            foreach (string linha in linhas)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Ignora a primeira linha que é o cabeçalho
                }

                emailBody += "<tr>";
                string[] colunas = linha.Split(',');

                foreach (string coluna in colunas)
                {
                    emailBody += $"<td>{coluna}</td>";
                }

                emailBody += "</tr>";
            }

            emailBody += @"
                        </tbody>
                    </table>
                    <div class='footer'>
                        <p>Este e-mail foi enviado automaticamente. Caso tenha dúvidas, entre em contato com nosso suporte.</p>
                    </div>
                </div>
            </body>
            </html>";

            // Dados do servidor SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "seu email";
            string smtpPassword = "sua senha smtp";
            string fixedRecipient = "destinatario";

            // Enviar o e-mail
            try
            {
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "Relatório de Produtos",
                    Body = emailBody,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(fixedRecipient);

                smtpClient.Send(mailMessage);
                Console.WriteLine("E-mail enviado com sucesso!");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Erro de SMTP: " + smtpEx.Message);
            }
            catch (FormatException formatEx)
            {
                Console.WriteLine("Erro de Formato: " + formatEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message);
            }
        }
    }
}
