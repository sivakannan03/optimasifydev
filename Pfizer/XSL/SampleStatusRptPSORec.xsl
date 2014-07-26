<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr class="tableContent1" align="center">
				<th>Date</th>
				<th>Product</th>
				<th>Sample Pack</th>
				<th class="tableheader">Allocated Date</th>
				<th class="tableheader">Allocated Qty</th>
				<th class="tableheader">Acknowledge Qty</th>
				<th class="tableheader">Reason for Short fall</th>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Rec']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Rec']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr>
						<td align="center" class="alerttext" colspan="7">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Rec']">
	<tr class="tablecontent1">
		<td align="left">
			<xsl:value-of select="@SBDate" />
		</td>
		<td align="left">
			<xsl:value-of select="@ProductName" />
		</td>
		<td align="left">
			<xsl:value-of select="@SamplePack" />
		</td>
		<td align="left">
			<xsl:value-of select="@SBAckDate" />
		</td>
		<td align="left">
			<xsl:value-of select="@AllocatedQty" />
		</td>
		<td align="left">
			<xsl:value-of select="@AcknowledgeQty" />
		</td>
		<td align="left">
			<xsl:value-of select="@ReasonForShortFall" />
		</td>
</tr>
</xsl:template>
</xsl:stylesheet>