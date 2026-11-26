<div align="center">

<img src="Assets/BioTrilha/Art/2D/Logo/Logo.png" alt="BioTrilha Logo" width="300"/>

# BioTrilha

*A digital board game for teaching Brazilian Bioeconomy*

![Static Badge](https://img.shields.io/badge/Unity-FFFFFF?style=flat&logo=unity&logoColor=white&labelColor=black)
![Static Badge](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=239120&labelColor=black)
![Static Badge](https://img.shields.io/badge/Android-34A853?style=flat&logo=android&logoColor=34A853&labelColor=black)
![Static Badge](https://img.shields.io/badge/Rider-000000?style=flat&logo=rider&logoColor=white&labelColor=black)
![Static Badge](https://img.shields.io/badge/Git-F05032?style=flat&logo=git&logoColor=F05032&labelColor=black)
![GitHub Repo stars](https://img.shields.io/github/stars/amiralo01/BioTrilha?style=flat&logo=github&logoColor=white&labelColor=black)
![GitHub last commit](https://img.shields.io/github/last-commit/amiralo01/BioTrilha?style=flat&logo=github&logoColor=white&labelColor=black)
![License](https://img.shields.io/github/license/amiralo01/BioTrilha?style=flat&labelColor=black)

</div>

---

## About the Project

**BioTrilha** is a digital board game developed for **Android mobile devices**, designed to promote environmental education and awareness about **Brazilian Bioeconomy** — covering themes such as sustainable products, biodiversity, and the economic potential of Brazil's natural biomes.

This project is the result of **years of research** and is being developed as a **Bachelor's thesis (TCC)** for the Computer Science degree at **UFOPA (Universidade Federal do Oeste do Pará)**. It is backed by a full academic article containing all game rules, visual models, and theoretical foundations.

> The game is currently in active development. The academic article is complete and serves as the primary reference for all game design decisions.

---

## Game Elements

The game features a rich set of physical and digital components:

| Element | Description |
|---|---|
| **Board** | 2D illustrated board with paths and biome regions |
| **Lucky / Penalty Cards** | Cards that trigger positive or negative events |
| **Question Cards** | Bioeconomy-themed trivia questions |
| **Product Cards** | Cards representing real Bioeconomic products from Brazilian biomes |
| **Action Cards** | Cards that alter the flow of the game |
| **Game Bills** | In-game currency in denominations of **5,000 · 10,000 · 50,000 · 100,000** |
| **Pawns** | 3D character models that move across the board |

---

## Screenshots

<div align="center">
<img src="Assets/Screenshots/estado-atual-tabuleiro.png" alt="Current board prototype in the Unity Game view" width="800"/>
<br/>
<sub>Current board prototype (Unity Game view, landscape, full-board camera): path with colored houses, START, FINISH, Penitentiary, the dice and the three pawns standing on the board</sub>
<br/><br/>
<img src="Assets/Screenshots/peoes-no-tabuleiro.png" alt="The three 3D pawns on the board" width="800"/>
<br/>
<sub>The three 3D pawns on the board: Engenheiro Agrônomo (green), Engenheiro Químico (yellow) and Cientista (blue, with the new hair)</sub>
<br/><br/>
<img src="Assets/Screenshots/cartas-do-jogo.png" alt="One card of each type, front and back" width="800"/>
<br/>
<sub>One card of each type, front and back: Action, Luck or Setback, Question and Bioeconomic Product</sub>
</div>

---

## Current Status

- [x] Academic article written with full game design documentation
- [x] Game logo finalized
- [x] Board artwork (2D) added
- [x] 3D dice model added
- [x] 3D pawn models added: Engenheiro Agrônomo, Engenheiro Químico, Cientista
- [x] Action card artwork (36 cards, front and back, transparent PNG)
- [x] Luck or Setback card artwork (26 cards and a shared back, transparent PNG)
- [x] Question card artwork (47 cards and a shared back, transparent PNG)
- [x] Product card artwork (75 cards, front and back, transparent PNG; each product has a category ID from 1 to 11)
- [x] Banknote artwork (5.000, 10.000, 50.000 and 100.000, transparent PNG)
- [x] Unity project initialized with Android mobile configuration
- [x] Project folder structure organized
- [x] Scene folders organized: Main Menu, Standard Game, Tutorial (scenes still to be built)
- [x] Board prototype scene (`SampleScene`): board, dice and pawns
- [ ] Game logic implementation (C# scripts)
- [x] Board path (71 houses + START and FINISH, read from the board art), dice roll and pawn movement house by house
- [x] Turn loop prototype: who starts (highest roll, ties roll again), turn order, stop at FINISH, second roll for the card level on green/blue houses
- [ ] Card delivery by house color (next step)
- [x] Action, Luck/Setback, Question and Product card data (ScriptableObjects) and card UI prefabs
- [ ] Card system and deck mechanics
- [ ] In-game currency system
- [x] Board camera controller (follows the moving pawn, then shows the full board)
- [ ] Dice roll and pawn movement on the board (3D)
- [ ] Tutorial scene with animated characters (2D)
- [ ] UI/UX for mobile screens
- [ ] Audio: soundtrack and sound effects
- [ ] Build and testing on Android devices
- [ ] Full documentation (in progress)

---

## Tech Stack

| Tool | Purpose |
|---|---|
| **Unity** | Game engine (2D + 3D hybrid) |
| **C#** | Game scripting language |
| **Android** | Target platform |
| **JetBrains Rider** | Code editor (IDE) |
| **Git + GitHub** | Version control and collaboration |

---

## Project Structure

```
Assets/
├── BioTrilha/
│   ├── Animations/            # Character and pawn animations
│   ├── Art/
│   │   ├── 2D/                # Board, cards, banknotes, logo, UI sprites
│   │   │   ├── Banknotes/     # 4 banknotes (Nota_5000 ... Nota_100000)
│   │   │   ├── BoardGame/     # Game board artwork
│   │   │   ├── Cards/
│   │   │   │   ├── Actions/   # 36 action cards (Front/ and Back/)
│   │   │   │   ├── LuckSetback/ # 26 Luck/Setback cards (Front/) + shared back (Back/)
│   │   │   │   ├── Questions/ # 47 Question cards (Front/) + shared back (Back/)
│   │   │   │   └── Products/  # 75 product cards (Front/ and Back/)
│   │   │   ├── Logo/          # Game logo
│   │   │   ├── Tutorial/
│   │   │   └── UI/
│   │   └── 3D/                # Dice and pawn models/textures
│   │       ├── Dice/
│   │       └── Pawns/
│   │           ├── Models/    # Engenheiro_Agronomo, Engenheiro_Químico, Cientista
│   │           └── Textures/
│   ├── Audio/                 # Music and sound effects
│   ├── Prefabs/                # Reusable game objects (Cards/ActionCard, LuckSetbackCard, QuestionCard, ProductCard)
│   ├── SciptableObjects/       # Game data (Cards/Actions 36, LuckSetback 26, Questions 47, Products 75)
│   └── Scripts/                # C# game logic (Banknotes, Board_Game, Cards, Editor, GameManager, Pawns, Tutorial, UI)
├── Scenes/
│   ├── SampleScene             # Current board prototype (board, dice, pawns)
│   ├── Main_menu               # Folder for the main menu scene (planned)
│   ├── Standardgame            # Folder for the main board game scene (planned)
│   └── Tutorial                # Folder for the tutorial scene (planned)
└── Screenshots/                 # Development preview captures
```

---

## Contributors

<div align="center">

<!-- Author -->
<table>
  <tr>
    <td align="center">
      <a href="https://github.com/amiralo01">
        <img src="https://github.com/amiralo01.png" width="100px;" alt="Amiraldo Ferreira"/><br/>
        <sub><b>Amiraldo Ferreira</b></sub>
      </a><br/>
      <sub>Author · Developer</sub>
    </td>
    <!-- Add co-author and collaborators below following the same pattern -->
    <!-- Example:
    <td align="center">
      <a href="https://github.com/USERNAME">
        <img src="https://github.com/USERNAME.png" width="100px;" alt="Name"/><br/>
        <sub><b>Full Name</b></sub>
      </a><br/>
      <sub>Co-author · Advisor</sub>
    </td>
    -->
  </tr>
</table>

</div>

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---
---

<div align="center">

<img src="Assets/BioTrilha/Art/2D/Logo/Logo.png" alt="BioTrilha Logo" width="300"/>

# BioTrilha

*Um jogo de tabuleiro digital para o ensino da Bioeconomia Brasileira*

![Static Badge](https://img.shields.io/badge/Unity-FFFFFF?style=flat&logo=unity&logoColor=white&labelColor=black)
![Static Badge](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=239120&labelColor=black)
![Static Badge](https://img.shields.io/badge/Android-34A853?style=flat&logo=android&logoColor=34A853&labelColor=black)
![Static Badge](https://img.shields.io/badge/Rider-000000?style=flat&logo=rider&logoColor=white&labelColor=black)
![Static Badge](https://img.shields.io/badge/Git-F05032?style=flat&logo=git&logoColor=F05032&labelColor=black)
![GitHub Repo stars](https://img.shields.io/github/stars/amiralo01/BioTrilha?style=flat&logo=github&logoColor=white&labelColor=black)
![GitHub last commit](https://img.shields.io/github/last-commit/amiralo01/BioTrilha?style=flat&logo=github&logoColor=white&labelColor=black)
![License](https://img.shields.io/github/license/amiralo01/BioTrilha?style=flat&labelColor=black)

</div>

---

## Sobre o Projeto

**BioTrilha** é um jogo de tabuleiro digital desenvolvido para **dispositivos móveis Android**, com o objetivo de promover a educação ambiental e a conscientização sobre a **Bioeconomia Brasileira** — abordando temas como produtos sustentáveis, biodiversidade e o potencial econômico dos biomas naturais do Brasil.

Este projeto é resultado de **anos de pesquisa** e está sendo desenvolvido como **Trabalho de Conclusão de Curso (TCC)** para o Bacharelado em Ciências da Computação na **UFOPA (Universidade Federal do Oeste do Pará)**. O projeto é sustentado por um artigo acadêmico completo contendo todas as regras do jogo, modelos visuais e fundamentação teórica.

> O jogo está atualmente em desenvolvimento ativo. O artigo acadêmico está concluído e serve como referência principal para todas as decisões de design do jogo.

---

## Elementos do Jogo

O jogo conta com um conjunto rico de componentes físicos e digitais:

| Elemento | Descrição |
|---|---|
| **Tabuleiro** | Tabuleiro ilustrado em 2D com trilhas e regiões de biomas |
| **Cartas de Sorte / Revés** | Cartas que ativam eventos positivos ou negativos |
| **Cartas de Questões** | Perguntas temáticas sobre Bioeconomia |
| **Cartas de Produtos** | Cartas representando produtos Bioeconômicos reais dos biomas brasileiros |
| **Cartas de Ação** | Cartas que alteram o fluxo do jogo |
| **Cédulas do Jogo** | Moeda do jogo nos valores de **5.000 · 10.000 · 50.000 · 100.000** |
| **Peões** | Modelos 3D de personagens que se movem pelo tabuleiro |

---

## Capturas de Tela

<div align="center">
<img src="Assets/Screenshots/estado-atual-tabuleiro.png" alt="Protótipo atual do tabuleiro na Game View do Unity" width="800"/>
<br/>
<sub>Protótipo atual do tabuleiro (Game View do Unity, modo paisagem, câmera com o tabuleiro inteiro): trilha com casas coloridas, INÍCIO, FIM, Penitenciária, o dado e os três peões sobre o tabuleiro</sub>
<br/><br/>
<img src="Assets/Screenshots/peoes-no-tabuleiro.png" alt="Os três peões 3D sobre o tabuleiro" width="800"/>
<br/>
<sub>Os três peões 3D sobre o tabuleiro: Engenheiro Agrônomo (verde), Engenheiro Químico (amarelo) e Cientista (azul, com o novo cabelo)</sub>
<br/><br/>
<img src="Assets/Screenshots/cartas-do-jogo.png" alt="Uma carta de cada tipo, frente e verso" width="800"/>
<br/>
<sub>Uma carta de cada tipo, frente e verso: Ação, Sorte ou Revés, Questões e Produto Bioeconômico</sub>
</div>

---

## Status Atual

- [x] Artigo acadêmico escrito com documentação completa do design do jogo
- [x] Logo do jogo finalizada
- [x] Arte do tabuleiro (2D) adicionada
- [x] Modelo 3D do dado adicionado
- [x] Modelos 3D dos peões adicionados: Engenheiro Agrônomo, Engenheiro Químico, Cientista
- [x] Arte das cartas de Ação (36 cartas, frente e verso, PNG transparente)
- [x] Arte das cartas de Sorte ou Revés (26 cartas e um verso comum, PNG transparente)
- [x] Arte das cartas de Questões (47 cartas e um verso comum, PNG transparente)
- [x] Arte das cartas de Produto (75 cartas, frente e verso, PNG transparente; cada produto tem um ID de categoria de 1 a 11)
- [x] Arte das cédulas (5.000, 10.000, 50.000 e 100.000, PNG transparente)
- [x] Projeto Unity inicializado com configuração mobile Android
- [x] Estrutura de pastas do projeto organizada
- [x] Pastas das cenas organizadas: Menu Principal, Jogo Padrão, Tutorial (cenas ainda a construir)
- [x] Cena de protótipo do tabuleiro (`SampleScene`): tabuleiro, dado e peões
- [ ] Implementação da lógica do jogo (scripts C#)
- [x] Trilha do tabuleiro (71 casas + INÍCIO e FIM, lidas da arte do tabuleiro), lançamento do dado e movimento do peão casa a casa
- [x] Protótipo do turno: quem começa (maior número, empate lança de novo), ordem dos turnos, parada no FIM, novo lançamento do nível da carta nas casas verde e azul
- [ ] Entrega das cartas conforme a cor da casa (próxima etapa)
- [x] Dados das cartas de Ação, de Sorte ou Revés, de Questões e de Produto (ScriptableObjects) e prefabs de UI das cartas
- [ ] Sistema de cartas e mecânica de baralho
- [ ] Sistema de moeda do jogo
- [x] Controlador de câmera do tabuleiro (segue o peão em movimento e depois mostra o tabuleiro inteiro)
- [ ] Rolagem do dado e movimentação dos peões no tabuleiro (3D)
- [ ] Cena de tutorial com personagens animados (2D)
- [ ] UI/UX para telas mobile
- [ ] Áudio: trilha sonora e efeitos sonoros
- [ ] Build e testes em dispositivos Android
- [ ] Documentação completa (em andamento)

---

## Tecnologias Utilizadas

| Ferramenta | Finalidade |
|---|---|
| **Unity** | Engine do jogo (híbrido 2D + 3D) |
| **C#** | Linguagem de programação do jogo |
| **Android** | Plataforma alvo |
| **JetBrains Rider** | Editor de código (IDE) |
| **Git + GitHub** | Controle de versão e colaboração |

---

## Estrutura do Projeto

```
Assets/
├── BioTrilha/
│   ├── Animations/            # Animações de personagens e peões
│   ├── Art/
│   │   ├── 2D/                # Tabuleiro, cartas, cédulas, logo, sprites de UI
│   │   │   ├── Banknotes/     # 4 cédulas (Nota_5000 ... Nota_100000)
│   │   │   ├── BoardGame/     # Arte do tabuleiro
│   │   │   ├── Cards/
│   │   │   │   ├── Actions/   # 36 cartas de Ação (Front/ e Back/)
│   │   │   │   ├── LuckSetback/ # 26 cartas de Sorte ou Revés (Front/) + verso comum (Back/)
│   │   │   │   ├── Questions/ # 47 cartas de Questões (Front/) + verso comum (Back/)
│   │   │   │   └── Products/  # 75 cartas de Produto (Front/ e Back/)
│   │   │   ├── Logo/          # Logo do jogo
│   │   │   ├── Tutorial/
│   │   │   └── UI/
│   │   └── 3D/                # Modelos e texturas do dado e dos peões
│   │       ├── Dice/
│   │       └── Pawns/
│   │           ├── Models/    # Engenheiro_Agronomo, Engenheiro_Químico, Cientista
│   │           └── Textures/
│   ├── Audio/                 # Músicas e efeitos sonoros
│   ├── Prefabs/                # Objetos reutilizáveis do jogo (Cards/ActionCard, LuckSetbackCard, QuestionCard, ProductCard)
│   ├── SciptableObjects/       # Dados do jogo (Cards/Actions 36, LuckSetback 26, Questions 47, Products 75)
│   └── Scripts/                # Lógica C# do jogo (Banknotes, Board_Game, Cards, Editor, GameManager, Pawns, Tutorial, UI)
├── Scenes/
│   ├── SampleScene             # Protótipo atual (tabuleiro, dado, peões)
│   ├── Main_menu               # Pasta da cena do menu principal (planejada)
│   ├── Standardgame            # Pasta da cena principal do tabuleiro (planejada)
│   └── Tutorial                # Pasta da cena do tutorial (planejada)
└── Screenshots/                 # Capturas de desenvolvimento
```

---

## Colaboradores

<div align="center">

<!-- Autor -->
<table>
  <tr>
    <td align="center">
      <a href="https://github.com/amiralo01">
        <img src="https://github.com/amiralo01.png" width="100px;" alt="Amiraldo Ferreira"/><br/>
        <sub><b>Amiraldo Ferreira</b></sub>
      </a><br/>
      <sub>Autor · Desenvolvedor</sub>
    </td>
    <!-- Adicione coautor e colaboradores abaixo seguindo o mesmo padrão -->
    <!-- Exemplo:
    <td align="center">
      <a href="https://github.com/USERNAME">
        <img src="https://github.com/USERNAME.png" width="100px;" alt="Nome"/><br/>
        <sub><b>Nome Completo</b></sub>
      </a><br/>
      <sub>Coautor · Orientador</sub>
    </td>
    -->
  </tr>
</table>

</div>

---

## Licença

Este projeto está licenciado sob a **Licença MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.