using System;

public static class BiobrickBuilder {
	public static BioBrick createBioBrickFromData(BiobrickDataCount biobrickDataCount)
	{
		return createBioBrickFromData(biobrickDataCount.biobrickData, biobrickDataCount.count);
	}

	public static BioBrick createBioBrickFromData(BiobrickData biobrickData, int count = 1)
	{
		BioBrick res = null;
		switch (biobrickData.type) {
		case BioBrick.Type.PROMOTER:
			var promoterBiobrickData = (PromoterBiobrickData)biobrickData;
			res = new PromoterBrick(
				promoterBiobrickData.id,
				promoterBiobrickData.beta,
				promoterBiobrickData.formula,
				promoterBiobrickData.size,
				count);
			break;
		case BioBrick.Type.RBS:
			var rbsBiobrickData = (RBSBiobrickData)biobrickData;
			res = new RBSBrick (
				rbsBiobrickData.id,
				rbsBiobrickData.rbsFactor,
				rbsBiobrickData.size,
				count
			);
			break;
		case BioBrick.Type.GENE:
			var geneBiobrickData = (GeneBiobrickData)biobrickData;
			res = new GeneBrick (
				geneBiobrickData.id,
				geneBiobrickData.protein,
				geneBiobrickData.size,
				count
			);
			break;
		case BioBrick.Type.TERMINATOR:
			var terminatorBiobrickData = (TerminatorBiobrickData)biobrickData;
			res = new TerminatorBrick (
				terminatorBiobrickData.id,
				terminatorBiobrickData.terminatorFactor,
				terminatorBiobrickData.size,
				count
			);
			break;
		}
		return res;
	}
}
