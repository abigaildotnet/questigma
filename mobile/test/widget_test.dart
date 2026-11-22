import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:questigma/app/questigma_app.dart';
import 'package:questigma/cases/case_repository.dart';
import 'package:questigma/cases/case_summary.dart';

class FakeCaseRepository implements CaseRepository {
  FakeCaseRepository(this.load);

  final Future<List<CaseSummary>> Function() load;

  @override
  Future<List<CaseSummary>> listCases() => load();
}

void main() {
  testWidgets('shows loading while the request is pending', (tester) async {
    final pending = Completer<List<CaseSummary>>();
    await tester.pumpWidget(
      QuestigmaApp(repository: FakeCaseRepository(() => pending.future)),
    );
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete([]);
    await tester.pumpAndSettle();
  });

  testWidgets('shows the returned cases', (tester) async {
    final repository = FakeCaseRepository(
      () async => [
        const CaseSummary(
          id: '1',
          title: 'O caso piloto',
          synopsis: 'Um mistério público.',
        ),
      ],
    );
    await tester.pumpWidget(QuestigmaApp(repository: repository));
    await tester.pumpAndSettle();
    expect(find.text('O caso piloto'), findsOneWidget);
    expect(find.text('Um mistério público.'), findsOneWidget);
    expect(find.byIcon(Icons.menu_book_outlined), findsOneWidget);
  });

  testWidgets('shows an empty message', (tester) async {
    await tester.pumpWidget(
      QuestigmaApp(repository: FakeCaseRepository(() async => [])),
    );
    await tester.pumpAndSettle();
    expect(find.text('Nenhum caso disponível no momento.'), findsOneWidget);
  });

  testWidgets('shows an error and retries the request', (tester) async {
    var attempts = 0;
    final repository = FakeCaseRepository(() async {
      attempts++;
      if (attempts == 1) throw Exception('network unavailable');
      return [
        const CaseSummary(
          id: '2',
          title: 'Caso recuperado',
          synopsis: 'Sinopse',
        ),
      ];
    });
    await tester.pumpWidget(QuestigmaApp(repository: repository));
    await tester.pumpAndSettle();
    expect(
      find.text(
        'Não foi possível carregar os casos. Verifique sua conexão e tente novamente.',
      ),
      findsOneWidget,
    );
    await tester.tap(find.text('Tentar novamente'));
    await tester.pumpAndSettle();
    expect(attempts, 2);
    expect(find.text('Caso recuperado'), findsOneWidget);
  });
}
