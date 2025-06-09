using System;
using System.IO;
using System.Text;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para carregar arquivos assembly
    /// </summary>
    public class FileLoader
    {
        // Construtor
        public FileLoader()
        {
        }

        // Carrega um arquivo assembly
        public string LoadAssemblyFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        // Salva um arquivo assembly
        public void SaveAssemblyFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        // Carrega um arquivo binário
        public byte[] LoadBinaryFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

            return File.ReadAllBytes(filePath);
        }

        // Salva um arquivo binário
        public void SaveBinaryFile(string filePath, byte[] content)
        {
            File.WriteAllBytes(filePath, content);
        }

        // Verifica se um arquivo existe
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        // Obtém o diretório do arquivo
        public string GetDirectoryName(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        // Obtém o nome do arquivo sem extensão
        public string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        // Obtém a extensão do arquivo
        public string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }
    }
}