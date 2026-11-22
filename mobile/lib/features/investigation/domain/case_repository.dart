import 'case_summary.dart';

abstract interface class CaseRepository {
  Future<List<CaseSummary>> listCases();
}
