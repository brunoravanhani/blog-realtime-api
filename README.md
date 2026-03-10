# Blog Realtime Api

- Swagger: https://localhost:7054/swagger/index.html
- Web Socket: https://localhost:7054/ws

# Questões relacionadas a C#


1. Orientação a Objetos:
• Explique o conceito de herança múltipla e como C# aborda esse cenário.
    R: Herança multipla não é possível no C# ao usar classes, entretanto podemos ter esse comportamento (ou parecido) ao usar interfaces, dessa forma é possível implementar multiplas interfaces.
• Explique o polimorfismo em C# e forneça um exemplo prático de como ele
pode ser implementado.
    R: Polimorfismo pode ser implementado em C# de duas formas, criando multiplos métodos com o mesmo nome e parametros diferentes: `void Create(string name)` e `void Create(string name, string email)`. Também podemos implementar usando a herança, métodos com modificador `virtual` em uma classe pai, podem ser sobreescritos usando o modificador `override` em uma classe filha.
2. SOLID:
• Descreva o princípio da Responsabilidade Única (SRP) e como ele se aplica em
um contexto de desenvolvimento C#.
    R: Principio da responsabilidade unica diz que uma classe, método ou função devem ter apenas uma responsabilidade ou apenas uma razão de mudança, por exemplo: se temos uma classe que faz a leitura de um arquivo texto e salva em um banco de dados, é uma classe que tem 2 responsabilidade, o correto, de acordo com o SRP, deveriam ser refatoradas para duas classes cada uma com uma unica responsabilidade.
• Como o princípio da inversão de dependência (DIP) pode ser aplicado em um
projeto C# e como isso beneficia a manutenção do código?
    R: 
3. Entity Framework (EF):
• Como o Entity Framework gerencia o mapeamento de objetos para o banco de
dados e vice-versa?
• Como otimizar consultas no Entity Framework para garantir um desempenho
eficiente em grandes conjuntos de dados?
4. WebSockets:
• Explique o papel dos WebSockets em uma aplicação C# e como eles se
comparam às solicitações HTTP tradicionais.
• Quais são as principais considerações de segurança ao implementar uma
comunicação baseada em WebSockets em uma aplicação C#?
5. Arquitetura:
• Descreva a diferença entre arquitetura monolítica e arquitetura de
microsserviços. Qual seria sua escolha ao projetar uma aplicação C#?
• Como você escolheria entre a arquitetura de microsserviços e a arquitetura
monolítica ao projetar uma aplicação C# que precisa ser altamente escalável?