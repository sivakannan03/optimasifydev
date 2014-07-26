<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainerRec">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr align="center">
				<th>Date</th>
				<th>Input</th>
				<th>Allocated Date</th>
				<th>Allocated Qty</th>
				<th>Acknowledge Qty</th>	
          </tr>
      
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Rec']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Rec']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center"  class="tb-r1"  colspan="5">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Rec']">
	<tr >
		<td align="left" class="tb-r2">
			<xsl:value-of select="@INPDate" />
		</td>
		<td align="left" class="tb-r2">
			<xsl:value-of select="@InputName" />
		</td>
		<td align="left" class="tb-r2">
			<xsl:value-of select="@INPAckDate" />
		</td>
		<td align="left" class="tb-r2">
			<xsl:value-of select="@AllocatedQty" />
		</td>
		<td align="left" class="tb-r2">
			<xsl:value-of select="@AcknowledgeQty" />
		</td>
</tr>
</xsl:template>
</xsl:stylesheet>