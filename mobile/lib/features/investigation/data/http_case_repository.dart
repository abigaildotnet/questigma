import 'case_api_client.dart';
import '../domain/case_repository.dart';
import '../domain/case_summary.dart';

class HttpCaseRepository implements CaseRepository {
  const HttpCaseRepository(this.client);

  final CaseApiClient client;

  @override
  Future<List<CaseSummary>> listCases() => client.listCases();
}
