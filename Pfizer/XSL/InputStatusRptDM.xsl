<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr class="tableContent1" align="center">
				<th rowspan="2">Input</th>
				<th class="tableheader" colspan="4">Input</th>
			</tr>
			<tr align="center">
				<th>Opening Balance</th>				
				<th>Received</th>
				<th>Disbursed</th>
				<th>Closing Balance</th>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Op']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Op']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center" class="alerttext" colspan="5">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Op']">
	<tr class="tablecontent1">
		<td align="left" clas="tb-r2">
			<input type="hidden" id="hinpkid" >
				<xsl:attribute name="value">
					<xsl:value-of select="@SampleFKID" />
				</xsl:attribute>
			</input>
			<xsl:value-of select="@InputName" />
		</td>
		<td align="left" clas="tb-r2">
			<xsl:value-of select="@OpeningBalance" />
		</td>
		<td align="left" clas="tb-r2">
			<xsl:value-of select="@Received" />
		</td>
		<td align="left" clas="tb-r2">
			<xsl:value-of select="@Disbursed" />
		</td>
		<td align="left" clas="tb-r2">
			<xsl:value-of select="@ClosingBalance" />
		</td>
</tr>
</xsl:template>
</xsl:stylesheet>