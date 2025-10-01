# Parking Registration 

O sistema Parking Registration é essencial para o funcionamento completo da solução Pulse, ele
coordena todo o processo de registro de Pátios (Parkings) e os recursos principais para a efetivação
do mapeamento e organização do espaço: Gateways e Zonas (Zones).
Trata-se de uma API Restful desenvolvida em .NET com foco em boas práticas e automação.

---

## 📌 Descrição do Projeto

O **Pulse** é um sistema voltado para a **gestão inteligente de pátios**, oferecendo funcionalidades como:

- **Cálculo de gateways necessários**: com base na cobertura de cada dispositivo e na área disponível do pátio.  
- **Gerenciamento de zonas**: cada pátio é dividido em 4 zonas padrão, e o sistema sugere medidas ideais durante o cadastro do pátio.  
- **Controle de pátios**: permite cadastro, atualização, listagem e remoção de pátios, garantindo organização da estrutura.

---

## 🏗 Arquitetura

A escolha pela arquitetura em **camadas** foi feita para garantir **organização, manutenibilidade e escalabilidade** do projeto.  

Cada camada possui uma responsabilidade bem definida, permitindo maior desacoplamento e facilitando a evolução da aplicação:

- **API**: concentra apenas a exposição de endpoints RESTful e o retorno das respostas no formato correto (com HATEOAS e status codes adequados), mantendo essa camada limpa e sem lógica de negócio.  
- **Application**: atua como orquestradora, chamando serviços e coordenando o fluxo entre domínio e infraestrutura. Isso facilita a implementação de regras de negócio sem acoplamento direto à camada de apresentação ou persistência.  
- **Domain**: é o coração do sistema, onde ficam as entidades e regras de negócio. Essa camada não depende de outras, o que garante independência e testabilidade das regras de negócio.  
- **Infrastructure**: cuida do acesso a dados e integrações externas. Dessa forma, mudanças no banco de dados ou em provedores externos impactam apenas esta camada, sem afetar diretamente o domínio ou a API.  

Essa abordagem segue princípios do **Domain-Driven Design (DDD)** e **Clean Architecture**, assegurando que a lógica de negócio permaneça isolada e independente de tecnologias ou frameworks específicos.  

---

## 🔧 Tecnologias Utilizadas

- **.NET 8**
- **C#**
- **Entity Framework Core**
- **HATEOAS**
- **Swagger / OpenAPI**
- **Banco de dados Oracle**

---

## 🚀 Como Rodar a API

1. Clonar o repositório
```bash
git clone https://github.com/seu-usuario/pulse-api.git
cd pulse-api
```

2. Restaurar dependências
```bash
dotnet restore
```

3. Configurar string de conexão
No **appsettings.json**, configure sua conexão com o banco de dados:
```bash
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PulseDB;User Id=sa;Password=senha123;"
}
```

4. Aplicar migrations
```bash
dotnet ef database update
```

5. Rodar a API
```bash
dotnet run
```

6. Acessar documentação Swagger
```bash
http://localhost:5000/swagger
```

---

## 

