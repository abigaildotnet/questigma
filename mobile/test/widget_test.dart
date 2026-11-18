import 'package:flutter_test/flutter_test.dart';
import 'package:questigma/app/questigma_app.dart';

void main() {
  testWidgets('shows the Questigma home screen', (tester) async {
    await tester.pumpWidget(const QuestigmaApp());
    expect(find.text('Questigma'), findsNWidgets(2));
  });
}
