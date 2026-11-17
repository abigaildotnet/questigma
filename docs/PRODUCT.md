# Questigma — visão do produto

## Visão e problema

Questigma é um jogo mobile de investigação de mistérios fictícios. O jogador reúne evidências, conversa com suspeitos e formula deduções para resolver casos. Atividades pessoais reais, como leitura, desenho, estudo de idiomas e exercícios, podem inspirar missões complementares, sem substituir a investigação no jogo.

Jogos de mistério costumam terminar na tela; ferramentas de hábitos, por sua vez, podem transformar interesses pessoais em obrigações. Questigma propõe uma ligação leve entre os dois: dar contexto lúdico a atividades que a pessoa já deseja fazer, preservando um jogo completo para quem prefere apenas investigar.

## Público-alvo e personas

- **Investigadora casual:** gosta de histórias de mistério no celular, joga em sessões curtas e quer concluir um caso sem depender de tarefas fora do jogo.
- **Investigadora com interesses pessoais:** também lê, desenha, estuda ou se exercita e gostaria de associar essas atividades à história, mas precisa poder ignorar missões em dias ocupados.

O produto atende pessoas que buscam entretenimento narrativo acessível, com ou sem interesse em conectar atividades da vida real à experiência.

## Proposta de valor e princípios

- **Investigação completa no jogo:** cada caso tem começo, evidências suficientes, deduções e desfecho acessíveis exclusivamente por ações dentro do jogo. A solução e as pistas essenciais são definidas pelo conteúdo autoral do caso.
- **Atividades reais por escolha:** o jogador pode aceitar, ignorar ou abandonar missões opcionais. Não há streaks, prazo obrigatório, perda de progresso, punição ou vantagem necessária para resolver o caso. Sua ausência não bloqueia pistas, interrogatórios, deduções ou progressão narrativa.
- **Complemento narrativo:** missões reais podem render contextualização, falas ou registros extras não essenciais. Deve existir um caminho equivalente dentro do jogo para compreender e concluir o caso, sem exigir explicações externas.
- **IA com papel limitado:** a IA pode sugerir ou adaptar a apresentação de missões opcionais a interesses escolhidos pelo jogador, dentro de modelos e limites definidos pela autora. Ela não determina a verdade de um caso, não cria ou altera evidências canônicas, não valida a solução e não controla o acesso a pistas essenciais nem a progressão narrativa. O jogo continua funcional se a IA estiver indisponível.

## Jornada principal

1. O jogador escolhe um caso e recebe sua premissa e objetivo no próprio jogo.
2. Explora cenas, reúne evidências e interroga suspeitos. O diário do caso apresenta o que foi descoberto e as possibilidades de investigação.
3. Relaciona evidências, testa hipóteses e avança por marcos narrativos definidos pelo caso.
4. Se quiser, escolhe um interesse e inicia uma missão ligada a uma atividade real. Pode registrar sua realização para receber conteúdo complementar, sem afetar os marcos obrigatórios.
5. Formula a dedução final com base nas evidências disponíveis no jogo e recebe o desfecho e uma explicação da solução.

O fluxo de interface deve explicar objetivos, ações disponíveis e consequências de cada dedução, de modo que o jogador não precise consultar instruções fora do jogo.

## MVP — escopo

O primeiro lançamento valida a investigação e a opção híbrida com um caso curto, completo e escrito manualmente. Para manter a entrega viável para uma desenvolvedora individual, o MVP inclui:

- Uma jornada inicial breve integrada ao caso, sem manual externo.
- Um caso autoral com cenas, evidências fixas, diálogos de suspeitos, diário, deduções e desfecho.
- Progressão determinística baseada apenas em ações e evidências do jogo, com verificação de que o caso pode ser concluído sem iniciar missão real.
- Um conjunto pequeno de missões opcionais predefinidas, associadas a interesses como leitura, desenho, idiomas ou exercício, com registro voluntário de início e conclusão e recompensa narrativa não essencial.
- Preferência simples para mostrar ou ocultar missões reais; a escolha pode ser alterada sem reiniciar o caso.
- Instrumentação mínima e respeitosa da privacidade para observar início de caso, início voluntário de missão, conclusão do caso e dificuldades no fluxo, sem coletar detalhes sensíveis da atividade real.

A personalização por IA fica fora da dependência do MVP: modelos de missão predefinidos bastam para testar a proposta híbrida. O contrato de limites da IA acima orienta qualquer implementação posterior.

## Fora do MVP e possibilidades futuras

Ficam fora do MVP: geração automática de casos ou soluções, conteúdo narrativo essencial gerado por IA, validação automática de atividades reais, integração com sensores ou aplicativos de saúde, recursos sociais, rankings, streaks, economia de recompensas e catálogo amplo de casos.

Após validar o caso inicial, podem ser considerados novos casos autorais, mais tipos de missões opcionais, ajustes de acessibilidade e personalização de missões por IA sob os limites já definidos. Cada adição deve preservar a conclusão integral do caso dentro do jogo.

## Critérios de sucesso

- **Compreensão e autonomia:** em testes com pessoas do público-alvo, participantes conseguem explicar o objetivo, encontrar a próxima ação e concluir o caso sem ajuda ou explicações externas. Observar onde ficam travadas e revisar o fluxo.
- **Conclusão sem atividades reais:** testar o percurso do caso do início ao desfecho com missões desativadas; a taxa de conclusão desse grupo é acompanhada separadamente e não deve depender da adesão às missões.
- **Início voluntário de atividades:** medir quantas pessoas optam por ver missões e, entre elas, quantas iniciam ao menos uma missão. Uma baixa adesão é sinal para investigar interesse e clareza, nunca motivo para impor tarefas.
- **Experiência percebida:** perguntar após o caso se a investigação foi clara e satisfatória e se as missões pareceram uma escolha livre. Registrar comentários sobre pressão, bloqueios e repetição para orientar melhorias.

Essas medidas servem para aprender com um primeiro caso e uma amostra pequena; metas numéricas de retenção ou expansão devem ser definidas somente após observar uso real.
