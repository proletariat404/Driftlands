public static class ConditionChecker
{
	public static bool IsConditionMet(string condition)
	{
		if (string.IsNullOrEmpty(condition))
			return true;

		if (condition.StartsWith("require:"))
		{
			string traitId = condition.Substring("require:".Length);
			return PlayerStats.HasTrait(traitId); // ?? ����Ҫʵ���������
		}

		return false;
	}
}
