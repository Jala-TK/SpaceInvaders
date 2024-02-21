# Space Invaders MVVM

## Descrição

Este é um projeto de jogo Space Invaders desenvolvido em C# usando o padrão de arquitetura MVVM (Model-View-ViewModel), a biblioteca Avalonia para a interface gráfica e e a LibVLCSharp para reprodução de áudio. O jogo possui funcionalidades básicas, como movimentação do jogador, atirar, inimigos que se movem e atiram, barreiras de proteção e contagem de pontuação.

## Estrutura do Projeto

- **SpaceInvadersMVVM**: Projeto principal contendo a lógica do jogo.
- **SpaceInvadersMVVM.ViewModels**: Classes de ViewModel para gerenciar a lógica da interface de usuário.
- **SpaceInvadersMVVM.Views**: Janelas e controles Avalonia XAML.
- **SpaceInvadersMVVM.Models**: Classes de modelo representando entidades do jogo.
- **Assets**: Recursos como imagens e áudios usados no jogo.

## Como Executar

1. Certifique-se de ter o .NET SDK instalado em sua máquina. Você pode baixá-lo em [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
2. Clone ou faça o download deste repositório.

   ```bash
   git clone https://github.com/Kainanars/SpaceInvaders.git
   ```

3. Navegue até o diretório do projeto.

   ```bash
   cd SpaceInvaders
   ```

4. Restaure as dependências e construa o projeto.

   ```bash
   dotnet restore
   dotnet build
   ```

5. Execute o projeto.

   ```bash
   dotnet run
   ```

6. A janela do jogo será exibida e você poderá começar a jogar. Pressione "Enter" para iniciar o jogo.

## Dependências

- **Avalonia**: Biblioteca de interface gráfica multiplataforma. Instalada automaticamente via NuGet.
- **ReactiveUI**: Biblioteca para programação reativa em .NET. Instalada automaticamente via NuGet.
- **LibVLCSharp**: Biblioteca para manipulação de áudio. Instalada automaticamente via NuGet.

## Controles do Jogo

- Setas direcionais ou "A" e "D" para mover o jogador para a esquerda e direita.
- Barra de espaço para atirar.

## LibVLCSharp

A biblioteca LibVLCSharp é essencial para a reprodução de áudio. Certifique-se de instalar o SDK do .NET Core e a biblioteca libvlc no sistema antes de executar o aplicativo. Para informações detalhadas sobre a instalação da LibVLCSharp no Linux, consulte [este guia](https://github.com/videolan/libvlcsharp/blob/3.x/docs/linux-setup.md) e para Windows, consulte [este guia](https://www.nuget.org/packages/VideoLAN.LibVLC.Windows/)

### Pré-requisitos

- Instale o SDK do .NET Core.
- Instale a biblioteca libvlc no sistema. (Certifique-se de que as bibliotecas libvlc.so e libvlccore.so estão localizadas em /usr/lib. Caso contrário, ajuste a variável LD_LIBRARY_PATH.)

## Instruções de Uso

1. Instale o .NET Core SDK.
2. Instale a biblioteca libvlc no sistema.
3. Execute o aplicativo com o comando:

```bash
dotnet run
```

4. Use as teclas W, A, S e D para movimentar a nave na tela.

## Desenvolvedores

- [Lisandra Dias](github.com/@lisscodes)
- [Kainan Rodrigues](github.com/@kainanars)
- [Tauã Ferreira](github.com/@tauz-hub)

---

Divirta-se jogando o nosso Space Invaders! 🚀👾
