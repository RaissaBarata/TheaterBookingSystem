using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static int tamanho = 5;
    static Dictionary<string, string[,,]> reservasPorPeca = new Dictionary<string, string[,,]>();
    static string pecaSelecionada = "Nenhuma";

    static void Main()
    {
        CarregarReservas();

        int opcao;
        do
        {
            Console.Clear();
            Console.WriteLine("==== Sistema de Reservas do Teatro ====");
            Console.WriteLine("1 - Escolher Peça");
            Console.WriteLine("2 - Reservar Assento");
            Console.WriteLine("3 - Visualizar Lugares");
            Console.WriteLine("4 - Cancelar Reserva");
            Console.WriteLine("5 - Sair");
            Console.Write("Escolha uma opção: ");

            if (int.TryParse(Console.ReadLine(), out opcao))
            {
                switch (opcao)
                {
                    case 1:
                        EscolherPeca();
                        break;
                    case 2:
                        ReservarAssento();
                        SalvarReservas();
                        break;
                    case 3:
                        VisualizarAssentos();
                        break;
                    case 4:
                        CancelarReserva();
                        SalvarReservas();
                        break;
                    case 5:
                        Console.WriteLine("Saindo do sistema...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Entrada inválida! Digite um número.");
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        } while (opcao != 5);
    }

    static void EscolherPeca()
    {
        Console.Clear();
        Console.WriteLine("==== Escolha a Peça ====");
        string[] pecas = { "Nosferatu", "O Fantasma da Ópera", "Drácula", "Frankenstein", "A Assombração da Casa da Colina" };

        for (int i = 0; i < pecas.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {pecas[i]}");
        }

        Console.Write("Escolha o número da peça: ");
        if (int.TryParse(Console.ReadLine(), out int escolha) && escolha >= 1 && escolha <= pecas.Length)
        {
            pecaSelecionada = pecas[escolha - 1];

            if (!reservasPorPeca.ContainsKey(pecaSelecionada))
            {
                reservasPorPeca[pecaSelecionada] = new string[tamanho, tamanho, tamanho];
            }

            Console.WriteLine($"Peça selecionada: {pecaSelecionada}");
        }
        else
        {
            Console.WriteLine("Opção inválida! Nenhuma peça foi selecionada.");
        }
    }

    static void ReservarAssento()
    {
        if (pecaSelecionada == "Nenhuma")
        {
            Console.WriteLine("Erro: Escolha uma peça antes de reservar um assento!");
            return;
        }

        Console.Clear();
        Console.WriteLine($"Peça atual: {pecaSelecionada}");
        Console.Write("Digite seu nome: ");
        string nome = Console.ReadLine();

        int setor, fileira, cadeira;
        if (PedirLocalizacao(out setor, out fileira, out cadeira))
        {
            if (reservasPorPeca[pecaSelecionada][setor, fileira, cadeira] == null)
            {
                reservasPorPeca[pecaSelecionada][setor, fileira, cadeira] = nome;
                Console.WriteLine($"Reserva feita com sucesso para {nome} na peça {pecaSelecionada}!");
            }
            else
            {
                Console.WriteLine("Erro: Esse assento já está ocupado!");
            }
        }
    }

    static void VisualizarAssentos()
    {
        if (pecaSelecionada == "Nenhuma")
        {
            Console.WriteLine("Erro: Escolha uma peça antes de visualizar assentos!");
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== Mapa de Assentos ({pecaSelecionada}) ===");

        for (int setor = 0; setor < tamanho; setor++)
        {
            Console.WriteLine($"\nSetor {setor + 1}:");
            for (int fileira = 0; fileira < tamanho; fileira++)
            {
                for (int cadeira = 0; cadeira < tamanho; cadeira++)
                {
                    Console.Write(reservasPorPeca[pecaSelecionada][setor, fileira, cadeira] == null ? "[ ] " : "[X] ");
                }
                Console.WriteLine();
            }
        }
    }

    static void CancelarReserva()
    {
        if (pecaSelecionada == "Nenhuma")
        {
            Console.WriteLine("Erro: Escolha uma peça antes de cancelar uma reserva!");
            return;
        }

        Console.Clear();
        Console.WriteLine("=== Cancelamento de Reserva ===");

        int setor, fileira, cadeira;
        if (PedirLocalizacao(out setor, out fileira, out cadeira))
        {
            if (reservasPorPeca[pecaSelecionada][setor, fileira, cadeira] != null)
            {
                Console.WriteLine($"Reserva de {reservasPorPeca[pecaSelecionada][setor, fileira, cadeira]} cancelada.");
                reservasPorPeca[pecaSelecionada][setor, fileira, cadeira] = null;
            }
            else
            {
                Console.WriteLine("Erro: Esse assento já está vazio.");
            }
        }
    }

    static bool PedirLocalizacao(out int setor, out int fileira, out int cadeira)
    {
        Console.Write("Digite o setor (1 a {0}): ", tamanho);
        setor = ObterEntradaValida() - 1;

        Console.Write("Digite a fileira (1 a {0}): ", tamanho);
        fileira = ObterEntradaValida() - 1;

        Console.Write("Digite a cadeira (1 a {0}): ", tamanho);
        cadeira = ObterEntradaValida() - 1;

        return (setor >= 0 && fileira >= 0 && cadeira >= 0);
    }

    static int ObterEntradaValida()
    {
        int valor;
        while (!int.TryParse(Console.ReadLine(), out valor) || valor < 1 || valor > tamanho)
        {
            Console.Write("Valor inválido! Digite um número entre 1 e {0}: ", tamanho);
        }
        return valor;
    }

    // 🔹 Salva reservas no arquivo JSON
    static void SalvarReservas()
    {
        string json = JsonConvert.SerializeObject(reservasPorPeca, Formatting.Indented);
        File.WriteAllText("reservas.json", json);
    }

    // 🔹 Carrega reservas do JSON
    static void CarregarReservas()
    {
        if (File.Exists("reservas.json"))
        {
            try
            {
                string json = File.ReadAllText("reservas.json");
                reservasPorPeca = JsonConvert.DeserializeObject<Dictionary<string, string[,,]>>(json);
            }
            catch
            {
                reservasPorPeca = new Dictionary<string, string[,,]>();
                Console.WriteLine("Erro ao carregar reservas. O arquivo pode estar corrompido.");
            }
        }
    }
}
