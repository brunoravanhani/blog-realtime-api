# Blog Realtime Api

- Swagger: https://localhost:7054/swagger/index.html
- Web Socket: https://localhost:7054/ws

Next steps

- [ ] Error handling
- [ ] Validation
- [ ] password crypto
- [ ] refactor Program.cs
- [ ] logs and observability

# Questões relacionadas a C#


1. Orientação a Objetos:
- Explique o conceito de herança múltipla e como C# aborda esse cenário.
    
    R: Herança multipla não é possível no C# ao usar classes, entretanto podemos ter esse comportamento (ou parecido) ao usar interfaces, dessa forma é possível implementar multiplas interfaces.

- Explique o polimorfismo em C# e forneça um exemplo prático de como ele
pode ser implementado.
    
    R: Polimorfismo pode ser implementado em C# de duas formas, criando multiplos métodos com o mesmo nome e parametros diferentes: `void Create(string name)` e `void Create(string name, string email)`. Também podemos implementar usando a herança, métodos com modificador `virtual` em uma classe pai, podem ser sobreescritos usando o modificador `override` em uma classe filha.

2. SOLID:
- Descreva o princípio da Responsabilidade Única (SRP) e como ele se aplica em
um contexto de desenvolvimento C#.
    
    R: Principio da responsabilidade unica diz que uma classe, método ou função devem ter apenas uma responsabilidade ou apenas uma razão de mudança, por exemplo: se temos uma classe que faz a leitura de um arquivo texto e salva em um banco de dados, é uma classe que tem 2 responsabilidade, o correto, de acordo com o SRP, deveriam ser refatoradas para duas classes cada uma com uma unica responsabilidade.

- Como o princípio da inversão de dependência (DIP) pode ser aplicado em um
projeto C# e como isso beneficia a manutenção do código?
    
    R: Inversão de dependencia afirma que as classes devem receber a instancia de suas dependencias, e não criar as instancias dentro da classe, isso ajuda a reduzir o acoplamento e facilita para realizar testes unitários

3. Entity Framework (EF):
- Como o Entity Framework gerencia o mapeamento de objetos para o banco de
dados e vice-versa?

    R: O EF guarda o estado dos objetos buscados de forma que seja possível realizar alterações necessárias e somente no fim salvar as alterações (executar commit).


- Como otimizar consultas no Entity Framework para garantir um desempenho
eficiente em grandes conjuntos de dados?
    
    R: Pode-se usar o Lazy loading. Também é necessário ter atenção ao momento de chamar os métodos como ToList() e First() pois são eles que, de fato, realizam as consulta, é importante ter cuidado com a quatidade de tabelas incluidas (o que no sql seriam o joins) pois é muito facil perder o controle do EF nesses casos.

1. WebSockets:
- Explique o papel dos WebSockets em uma aplicação C# e como eles se
comparam às solicitações HTTP tradicionais.
    
    R: As requisições normais fazer o que chamamos de HandShake, executam a operação e finalizama conexão. Já requisições com WebSockets fazem o HandShake, mas deixam a conexão aberta para a troca de informações mais rapida. Isso permite mais velcidade ao trocar diversos dados em sequencia entretando essa conexão aberta ocupa recursos so server.

- Quais são as principais considerações de segurança ao implementar uma
comunicação baseada em WebSockets em uma aplicação C#?
    
    R: Acredito que assim como em conexões http é necessaria usar o protocolo com segurança https ou wss e verificar token de autenticaçaõ/autorização.

1. Arquitetura:
- Descreva a diferença entre arquitetura monolítica e arquitetura de
microsserviços. Qual seria sua escolha ao projetar uma aplicação C#?

    R: Arquitetura monolítica tem apenas uma aplicação que recebe todas as requests, assim todas as funcionalidades então na mesma aplicação e mantém-se apenas uma base de código. Já na arquitetura de microsserviços têm-se multiplas aplicações pequenas que normalmente atendem a apenas um dominio do negocio, cada aplicação tem seu banco de dados e sua base de código separada, podendo inclusive serem de linguagens ou plataformas diferentes. 
    A minha escolha dependeria to tipo, complexidade e tamanho do sistema, assim como a carga de trabalho necessária. Um sistema com microsserviços dá mais possibilidades, mas aumenta a complexidade do desenvolvimento necessitando coisas como Api gateway, mensageria para a comunicação, gerenciamento de transações, etc.

- Como você escolheria entre a arquitetura de microsserviços e a arquitetura
monolítica ao projetar uma aplicação C# que precisa ser altamente escalável?

    R: Acredito que uma  aplicação altamente escalavel indica que necessite de uma arquitetura de microsserviços pois assim tem mais possibilidades de gerenciamento e controle sobre distribuição de carga, escalabilidade horizontal e sobre quais serviços escalar.