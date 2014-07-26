<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0"  class = "inner_tb" border="1">
			<tr  align="center">
				<th  rowspan="2">Product</th>
				<th  rowspan="2">Sample Pack</th>
				<th colspan="5">Samples</th>
			</tr>
			<tr  align="center">
				<th  >Allocated Quantity</th>
			
				<th  >Opening Balance</th>
				
				<th >
          Received
          <a class="link1mod">
						<xsl:attribute name="href">
							javascript:showReport('0')
						</xsl:attribute>
						
					</a>
				</th>
				<th >
          Disbursed
          <a class="link1mod">
						<xsl:attribute name="href">
							javascript:showReportDisbursed('0')
						</xsl:attribute>
						
					</a>
				</th>
				<th >Closing Balance</th>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Op']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Op']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center" class="alerttext" colspan="6">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Op']">
	<tr class="tb-r1">
		<td align="left">
			<input type="hidden" id="hinpkid" >
				<xsl:attribute name="value">
					<xsl:value-of select="@SampleFKID" />
				</xsl:attribute>
			</input>
			<xsl:value-of select="@ProductName" />
		</td>
		<td align="left">
			<xsl:value-of select="@SamplePack" />
		</td>
		<td align="left">
			<xsl:value-of select="@AllocatedQuantity" />
		</td>
		<td align="left">
			<xsl:value-of select="@OpeningBalance" />
		</td>
		<td align="left">
			<a class="link1" >
				<xsl:attribute name="href">
					javascript:showReport('<xsl:value-of select="@SampleFKID"/>')
				</xsl:attribute>
				<xsl:value-of select="@Received" />
			</a>
		</td>
		<td align="left">
			<a class="link1" >
				<xsl:attribute name="href">
					javascript:showReportDisbursed('<xsl:value-of select="@SampleFKID"/>')
				</xsl:attribute>
				<xsl:value-of select="@Disbursed" />
			</a>
		</td>
		<td align="left">
			<xsl:value-of select="@ClosingBalance" />
		</td>
</tr>
</xsl:template>
</xsl:stylesheet>