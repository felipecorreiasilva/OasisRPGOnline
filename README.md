# Oasis RPG Online - Client

Bem-vindo ao repositório do cliente do **Oasis RPG Online**. Este projeto é um motor de jogo em Unity focado em alta modularidade, utilizando uma arquitetura baseada em **MVC (Model-View-Controller)** para separar a lógica de rede, a interface do usuário e os serviços de processamento de entidades.

Este cliente foi desenvolvido para trabalhar em conjunto com o [oasis_emulator](https://github.com/felipecorreiasilva/oasis_emulator), que atua como a autoridade absoluta sobre o estado do jogo e regras de negócio.

## Arquitetura MVC (Camada App/)
A pasta `App/` orquestra a interação entre os sistemas:

* **Network/ (Packet Handlers):** Camada de *dispatchers* e *handlers* que processam os pacotes (`Clif` pattern) após serem recebidos pelo `NetworkManager`.
* **UI/ (View):** Camada de visualização. Contém a interface do usuário e janelas contextuais.
* **Controllers/ (Controller):** Orquestradores que fazem a "ponte". Eles recebem eventos da camada de rede, decidem qual lógica executar nos `Services/` e atualizam a `UI/`.

## Services/ (Cérebro do Motor)
Onde reside a "verdade" do jogo. Estes serviços processam dados brutos e gerenciam o estado do mundo.

* **Core/**: Infraestrutura base. Contém o `NetworkManager` (responsável pela conexão via socket) e o `DataManager` (responsável pelo carregamento de configurações como o `Assets/data/clientinfo.xml`).
* **Entities/ (BL - Block List)**: Gerenciadores de entidades (estilo rAthena):
    * `Character/`: `PCManager`, `MobManager`, `NpcManager`.
    * `Items/`: `ItemManager`.
    * `WorldObjects/`: `PortalManager`.
* **Combat/**: Regras de cálculo (`SkillManager`, `StatusManager` para `SC_`).
* **Effects/**: Regras de visualização (`EffectManager` para `EFST_` e `TooltipManager`).
* **Content/**: Conteúdo de progressão (`QuestManager`, `DatabaseManager`).

---

## Configuração de Conexão
O cliente utiliza o arquivo **`Assets/data/clientinfo.xml`** para definir as configurações de conexão (IP e Porta). O `DataManager` realiza a leitura automática deste arquivo durante a inicialização, garantindo que o `NetworkManager` saiba onde conectar sem a necessidade de valores fixos no código.

---

## Fluxo de Comunicação
1.  **Recepção:** O `NetworkManager` (`Services/Core/`) recebe dados via socket e encaminha para os Handlers na pasta `App/Network/`.
2.  **Processamento:** O `Controller` recebe o sinal do Handler, consulta os `Services/` (Model) para aplicar regras de negócio.
3.  **Visualização:** O `Controller` dispara um evento que a `UI/` (View) escuta, atualizando o que o jogador vê na tela.

---

## Convenções do Projeto
- **Padrão rAthena:** Nomenclaturas como `Clif`, `BL`, `Opcodes` seguem o padrão do emulador oficial.
- **Minúsculas:** Todos os nomes de pastas e arquivos seguem o padrão *lowercase* (`data/`, `luafiles/`).
- **BL (Block List):** Qualquer objeto com ID único de rede é uma `Entity` e deve residir em `Services/Entities/`.

---

## Como Contribuir
1. **Respeite o MVC:** A `UI/` nunca deve acessar a `Network/` diretamente. Use sempre um `Controller/` como intermediário.
2. **Encapsulamento:** Sistemas novos devem ser `Managers` dentro de `Services/`.
3. **Segurança:** O cliente não deve conter lógica de validação crítica. O servidor [oasis_emulator](https://github.com/felipecorreiasilva/oasis_emulator) é a autoridade absoluta.

---
*Desenvolvido para o ecossistema Oasis RPG.*