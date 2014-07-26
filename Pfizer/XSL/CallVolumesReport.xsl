<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr class="tableContent1" align="center">
				<th rowspan="2" align="center">TEAM</th>
				<th  rowspan="2">Month</th>
        <th rowspan="2">Region Name</th>
				<th rowspan="2">District Name</th>
				<th rowspan="2">Employee Name</th>
				<th rowspan="2">Product Name</th>				
				<th colspan="7">Actuals (as in daily reports)</th>
				<th colspan="7">PLan (as in detialing grid)</th>
			</tr>
			<tr class="tableContent1" align="center">
        <th>Detailed 1</th>
          <th>Detailed 2</th>
            <th>Detailed 3</th>
              <th>Detailed 4</th>
                <th>R1</th>
                  <th>R2</th>
                    <th>TOTAL Actual</th>
                      <th>Detailed 1</th>
                        <th>Detailed 2</th>
                          <th>Detailed 3</th>
                            <th>Detailed 4</th>
                              <th>R1</th>
                                <th>R1</th>
                                  <th>TOTAL PLAN</th>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='callvolumesReport']) &gt;0">
					<xsl:apply-templates select="//row[@Element='callvolumesReport']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center" class="alerttext" colspan="20">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='callvolumesReport']">
	<tr class="tablecontent1">
		<td align="center" class="tb-r2">
			<xsl:value-of select="@TeamName" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@Month" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@RegionName" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@DistrictName" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PSONAme" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ProductName" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualD1" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualD2" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualD3" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualD4" />
		</td>
		<td align="center">
			<xsl:value-of select="@ActualD5" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualD6" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@ActualTotal" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanD1" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanD2" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanD3" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanD4" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanR1" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanR2" />
		</td>
		<td align="center" class="tb-r2">
			<xsl:value-of select="@PlanTotal" />
		</td>
	</tr>
</xsl:template>
</xsl:stylesheet>