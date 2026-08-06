# Guia Passo-a-Passo: Aplicar as Correções

## 🎯 Objetivo
Resolver o problema de "Total de atletas: 0" na página de atletas da edição.

---

## 📋 Pré-requisitos
- MySQL/MariaDB instalado e em execução
- Acesso à base de dados `bdolimpicoJueGu`
- Credenciais: `root` / `12345678` (conforme configurado no projeto)

---

## ✅ Passo 1: Fazer Backup (Recomendado)

Se você já tem dados importantes no banco, faça um backup primeiro:

```bash
# No terminal/PowerShell
mysqldump -u root -p12345678 bdolimpicoJueGu > backup_antes_correcao.sql
```

---

## ✅ Passo 2: Escolher uma Estratégia

### Opção A: Reconstruir Completamente (Recomendado para Teste)
**Use se**: Está testando ou não tem dados importantes

```bash
# Executar o SQL corrigido (limpa e reconstrói tudo)
mysql -u root -p12345678 < bancoolimpicos_CORRIGIDO.sql
```

### Opção B: Apenas Adicionar Dados (Recomendado para Produção)
**Use se**: Já tem dados e quer apenas adicionar os novos registos

```bash
# Abra o MySQL
mysql -u root -p12345678 bdolimpicoJueGu

# Cole apenas os INSERTs das linhas 127-165 do arquivo corrigido
```

---

## ✅ Passo 3: Validar as Correções

Após executar o SQL, teste os seguintes comandos:

### Teste 1: Verificar Edições com Atletas
```sql
SELECT 
    e.codedicao,
    e.ano,
    e.sede,
    COUNT(DISTINCT r.codAtleta) AS total_atletas
FROM edicao e
LEFT JOIN resultadosatletas r ON e.codedicao = r.edicao
GROUP BY e.codedicao, e.ano, e.sede
HAVING total_atletas > 0
ORDER BY e.codedicao;
```

**Resultado esperado**: Deve listar edições 6, 7, 8, 9, 23, 24, 25, 26, 27

### Teste 2: Testar a Stored Procedure
```sql
CALL sp_GetAtletasByEdicao(25);
```

**Resultado esperado**: Deve retornar 7 atletas (Stephanie, Thaissa, Wanda, Manuel, Marcelo, Fofão, Rebeca)

### Teste 3: Testar Outras Edições
```sql
CALL sp_GetAtletasByEdicao(26);  -- 7 atletas
CALL sp_GetAtletasByEdicao(27);  -- 9 atletas
```

---

## ✅ Passo 4: Testar no Navegador

1. **Inicie o projeto**:
   ```bash
   cd ProjetoOlimpicos
   dotnet run
   ```

2. **Acesse a página de Edições**:
   - URL: `http://localhost:5000/Edicao/Index`

3. **Clique em "Ver Atletas" para a Edição 25 (2024 - Paris)**:
   - Deve aparecer: "Total de atletas: 7"
   - Deve listar os 7 atletas

4. **Teste o filtro de modalidade**:
   - Selecione "Atletismo"
   - Deve aparecer apenas atletas dessa modalidade

5. **Teste as outras edições**:
   - Edição 26 (2028): 7 atletas
   - Edição 27 (2032): 9 atletas

---

## ✅ Passo 5: Verificar Detalhes de um Atleta

1. **Na página de atletas da edição 25**
2. **Clique no nome de um atleta** (ex: "Rebeca Andrade")
3. **Deve aparecer**:
   - Dados pessoais (data de nascimento, sexo, cidade, estado)
   - Modalidade (Ginástica Artística)
   - Histórico de participações

---

## 🔍 Troubleshooting

### Problema: "Total de atletas: 0" continua aparecendo

**Solução 1**: Verifique se o SQL foi executado corretamente
```sql
SELECT COUNT(*) FROM resultadosatletas WHERE edicao = 25;
```
Deve retornar: `7`

**Solução 2**: Reinicie o projeto
```bash
# Parar (Ctrl+C)
# Reconstruir
dotnet clean
dotnet build

# Executar
dotnet run
```

**Solução 3**: Limpe o cache do navegador
- Pressione `Ctrl+Shift+Delete` (ou `Cmd+Shift+Delete` no Mac)
- Selecione "Limpar dados de navegação"

### Problema: Erro ao executar o SQL

**Erro**: "Syntax error near line X"
- Verifique se está usando MySQL 5.7+ ou MariaDB 10.2+
- Tente executar linha por linha

**Erro**: "Access denied"
- Verifique as credenciais: `root` / `12345678`
- Verifique se o MySQL está em execução

### Problema: Dados não aparecem na dropdown de edições

**Solução**: Verifique a conexão
```csharp
// No arquivo Data/Database.cs, linha 7
// Confirme que a connection string está correta:
// server=localhost;port=3306;database=bdolimpicoJueGu;user=root;password=12345678;
```

---

## 📊 Dados Inseridos

### Edição 25 (2024 - Paris)
| Atleta | Prova | Resultado | Medalha |
|--------|-------|-----------|---------|
| Aderval Luiz Arvani | 10000m Masculino | 5ºLugar | - |
| Stephanie Balduccini | 100m Feminino | 2ºLugar | Prata |
| Thaissa Barbosa Presti | 100m Masculino | 3ºLugar | Bronze |
| Wanda dos Santos | 100m com barreiras Feminino | 1ºLugar | Ouro |
| Manuel dos Santos Filho | 110m com Barreiras Masculino | 4ºLugar | - |
| Marcelo Teles Negrão | 1500m Masculino | 6ºLugar | - |
| Fofão | 200m Feminino | 7ºLugar | - |

### Edição 26 (2028 - Los Angeles)
| Atleta | Prova | Resultado | Medalha |
|--------|-------|-----------|---------|
| Adhemar Ferreira da Silva | 200m Masculino | 1ºLugar | Ouro |
| Aderval Luiz Arvani | 20km Marcha Atlética Feminina | 2ºLugar | Prata |
| Stephanie Balduccini | 20km Marcha Atlética Masculino | 3ºLugar | Bronze |
| Thaissa Barbosa Presti | 3000m com Obstáculos Feminino | 4ºLugar | - |
| Wanda dos Santos | 3000m com Obstáculos Masculino | 5ºLugar | - |
| Manuel dos Santos Filho | Salto sobre a mesa Feminino | 1ºLugar | Ouro |
| Marcelo Teles Negrão | Salto sobre a mesa Masculino | 2ºLugar | Prata |

### Edição 27 (2032 - Brisbane)
| Atleta | Prova | Resultado | Medalha |
|--------|-------|-----------|---------|
| Adhemar Ferreira da Silva | 400m Feminino | 1ºLugar | Ouro |
| Aderval Luiz Arvani | 400m Feminino Feminino | 2ºLugar | Prata |
| Stephanie Balduccini | 400m Masculino | 3ºLugar | Bronze |
| Thaissa Barbosa Presti | 400m com Barreiras Feminina | 4ºLugar | - |
| Wanda dos Santos | 400m com Barreiras Feminino | 5ºLugar | - |
| Manuel dos Santos Filho | 400m com Barreiras Masculino | 6ºLugar | - |
| Marcelo Teles Negrão | 5000m Feminino | 7ºLugar | - |
| Fofão | 5000m Masculino | 8ºLugar | - |
| Rebeca Andrade | Salto sobre a mesa Masculino | 1ºLugar | Ouro |

---

## ✨ Próximos Passos (Opcional)

Após validar as correções, você pode:

1. **Adicionar mais dados**:
   - Inserir resultados para as outras 18 edições
   - Criar mais atletas

2. **Melhorar a interface**:
   - Adicionar paginação
   - Adicionar busca por nome
   - Adicionar gráficos de medalhas

3. **Implementar validações**:
   - Verificar se edição existe
   - Mostrar mensagem se não há atletas
   - Validar duplicatas

---

## 📞 Suporte

Se encontrar algum problema:

1. Verifique o arquivo `DIAGNOSTICO_E_CORRECOES.md`
2. Verifique os logs do MySQL
3. Verifique os logs do projeto ASP.NET Core

---

## ✅ Checklist Final

- [ ] Backup feito (se necessário)
- [ ] SQL corrigido executado
- [ ] Testes SQL validados
- [ ] Projeto reiniciado
- [ ] Página de edições carrega corretamente
- [ ] Edição 25 mostra 7 atletas
- [ ] Edição 26 mostra 7 atletas
- [ ] Edição 27 mostra 9 atletas
- [ ] Filtro de modalidade funciona
- [ ] Clique em atleta mostra detalhes

---

**Parabéns! As correções foram aplicadas com sucesso! 🎉**
